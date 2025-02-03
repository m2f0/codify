"""Define a custom Reasoning and Action agent."""
from datetime import datetime, timezone
from typing import Dict, List, Literal, cast
from langchain_core.messages import AIMessage
from langchain_core.runnables import RunnableConfig
from langgraph.graph import StateGraph
from langgraph.prebuilt import ToolNode
from langsmith.run_helpers import traceable
from react_agent.configuration import Configuration
from react_agent.state import InputState, State
from react_agent.tools import TOOLS
from react_agent.utils import load_chat_model

# Define the function that calls the model


@traceable(
    name="call_model",
    run_type="llm",
    tags=["model_call", "reasoning"]
)
async def call_model(
    state: State,
    config: RunnableConfig
) -> Dict[str, List[AIMessage]]:
    """Call the LLM powering our 'agent'."""
    configuration = Configuration.from_runnable_config(config)
    model = load_chat_model(configuration.model).bind_tools(TOOLS)
    
    system_message = configuration.system_prompt.format(
        system_time=datetime.now(tz=timezone.utc).isoformat()
    )
    
    response = cast(
        AIMessage,
        await model.ainvoke(
            [{"role": "system", "content": system_message}, *state.messages], config
        ),
    )
    
    if state.is_last_step and response.tool_calls:
        return {
            "messages": [
                AIMessage(
                    id=response.id,
                    content="Sorry, I could not find an answer to your question in the specified number of steps.",
                )
            ]
        }
    
    return {"messages": [response]}


# Define a new graph

builder = StateGraph(State, input=InputState, config_schema=Configuration)

# Define the two nodes we will cycle between
builder.add_node("call_model", call_model)  # Nome explícito para o node
builder.add_node("tools", ToolNode(TOOLS, name="tool_executor"))  # Nome explícito para o ToolNode

# Set the entrypoint as `call_model`
# This means that this node is the first one called
builder.add_edge("__start__", "call_model")


@traceable(
    name="route_model_output",
    run_type="chain",
    tags=["routing", "decision"]
)
def route_model_output(state: State) -> Literal["__end__", "tools"]:
    """Determine the next node based on the model's output."""
    last_message = state.messages[-1]
    if not isinstance(last_message, AIMessage):
        raise ValueError(
            f"Expected AIMessage in output edges, but got {type(last_message).__name__}"
        )
    if not last_message.tool_calls:
        return "__end__"
    return "tools"


# Add a conditional edge to determine the next step after `call_model`
builder.add_conditional_edges(
    "call_model",
    # After call_model finishes running, the next node(s) are scheduled
    # based on the output from route_model_output
    route_model_output,
    {
        "__end__": "__end__",  # Changed from 'end_conversation' to '__end__'
        "tools": "tools"
    }
)

# Add a normal edge from `tools` to `call_model`
# This creates a cycle: after using tools, we always return to the model
builder.add_edge("tools", "call_model")

# Compile the builder into an executable graph
graph = builder.compile(
    interrupt_before=["call_model", "tools"],
    interrupt_after=["call_model", "tools"]
)
