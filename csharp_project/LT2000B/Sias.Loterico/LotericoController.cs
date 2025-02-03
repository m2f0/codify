
using Microsoft.AspNetCore.Mvc;

using Sias.Loterico.Model;
using IA_ConverterCommons;
using static Code.LT2000B;
using Code;

namespace Sias.Loterico
{
    [ApiController]
    [Route("Loterico")]
    public class LotericoController : ControllerBase
    {

        [HttpPost("LT2000B")]
        public IActionResult LT2000B(LT2000BModel LT2000BModel_P)
        {
            var program = new LT2000B();
            var result = program.Execute(LT2000BModel_P.CADASTRO_FILE_NAME, LT2000BModel_P.RLT2000B_FILE_NAME);
            return Ok(result);
        }

    }
}
