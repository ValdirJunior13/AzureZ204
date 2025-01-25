using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace httpValidaCpf
{
    public static class fndoisvalidacpf
    {
        [FunctionName("fndoisvalidacpf")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("Iniciando a validação do CPF");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);

            if (data == null || string.IsNullOrEmpty((string)data?.cpf))
            {
                return new BadRequestObjectResult("Por favor, informe o CPF no formato correto.");
            }

            string cpf = (string)data?.cpf;

            bool isValid = ValidaCPF(cpf);

            string responseMessage = isValid
                ? "O CPF informado é válido."
                : "O CPF informado é inválido.";

            return new OkObjectResult(responseMessage);
        }

        public static bool ValidaCPF(string cpf)
        {
            if (string.IsNullOrEmpty(cpf)) return false;

            cpf = cpf.Replace(".", "").Replace("-", "");

            if (cpf.Length != 11 || !long.TryParse(cpf, out _))
                return false;
            if (new string(cpf[0], cpf.Length) == cpf)
                return false;

            int[] multiplicadores1 = { 10, 9, 8, 7, 6, 5, 4, 3, 2 };
            int[] multiplicadores2 = { 11, 10, 9, 8, 7, 6, 5, 4, 3, 2 };

            string tempCpf = cpf.Substring(0, 9);
            int soma = 0;

            for (int i = 0; i < 9; i++)
                soma += int.Parse(tempCpf[i].ToString()) * multiplicadores1[i];

            int resto = (soma * 10) % 11;
            if (resto == 10 || resto == 11) resto = 0;

            if (resto != int.Parse(cpf[9].ToString()))
                return false;

            tempCpf += cpf[9];
            soma = 0;

            for (int i = 0; i < 10; i++)
                soma += int.Parse(tempCpf[i].ToString()) * multiplicadores2[i];

            resto = (soma * 10) % 11;
            if (resto == 10 || resto == 11) resto = 0;

            return resto == int.Parse(cpf[10].ToString());
        }
    }
}
