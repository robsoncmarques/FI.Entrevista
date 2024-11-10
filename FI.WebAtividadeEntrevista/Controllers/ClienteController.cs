using FI.AtividadeEntrevista.BLL;
using WebAtividadeEntrevista.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FI.AtividadeEntrevista.DML;
using System.Web.Configuration;

namespace WebAtividadeEntrevista.Controllers
{
    public class ClienteController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }


        public ActionResult Incluir()
        {
            return View();
        }


        [HttpPost]
        public JsonResult Incluir(ClienteModel model)
        {
            BoCliente bo = new BoCliente();
            BoBeneficiario boBeneficiario = new BoBeneficiario();

            if (!isCPFValido(model.CPF))
                this.ModelState.AddModelError("CPFCliente", "CPF " + model.CPF + " é inválido");

            if (bo.VerificarExistencia(model.CPF))
                this.ModelState.AddModelError("CPFCliente", "CPF " + model.CPF + " já existe como cliente!");

            if (model.Beneficiarios != null)
            {
                foreach (var beneficiario in model.Beneficiarios)
                {
                    if (!isCPFValido(beneficiario.CPF))
                        this.ModelState.AddModelError("CPFBeneficiario", "CPF " + beneficiario.CPF + " é inválido");

                    if (model.Beneficiarios.Any(x => x.CPF == beneficiario.CPF && x != beneficiario))
                        this.ModelState.AddModelError("CPFBeneficiario", "CPF " + beneficiario.CPF + " já existe para o mesmo cliente!");
                }
            }

            if (!this.ModelState.IsValid)
            {
                List<string> erros = (from item in ModelState.Values
                                      from error in item.Errors
                                      select error.ErrorMessage + Environment.NewLine).ToList();

                Response.StatusCode = 400;
                return Json(string.Join(Environment.NewLine, erros));
            }
            else
            {

                model.Id = bo.Incluir(new Cliente()
                {
                    CEP = model.CEP,
                    Cidade = model.Cidade,
                    Email = model.Email,
                    Estado = model.Estado,
                    Logradouro = model.Logradouro,
                    Nacionalidade = model.Nacionalidade,
                    Nome = model.Nome,
                    Sobrenome = model.Sobrenome,
                    Telefone = model.Telefone,
                    CPF = model.CPF
                });


                return Json("Cadastro efetuado com sucesso");
            }
        }

        [HttpPost]
        public JsonResult Alterar(ClienteModel model)
        {
            BoCliente bo = new BoCliente();
            BoBeneficiario boBeneficiario = new BoBeneficiario();

            if (!isCPFValido(model.CPF))
                this.ModelState.AddModelError("CPFCliente", "CPF " + model.CPF + " é inválido");

            foreach (var beneficiario in model.Beneficiarios)
            {
                if (!isCPFValido(beneficiario.CPF))
                    this.ModelState.AddModelError("CPFBeneficiario", "CPF " + beneficiario.CPF + " do beneficiário é inválido");

                if ((beneficiario.Id == 0 && 
                    boBeneficiario.VerificarExistencia(model.CPF, model.Id)) || 
                    model.Beneficiarios.Any(x => x.CPF == beneficiario.CPF && x != beneficiario))
                    this.ModelState.AddModelError("CPFBeneficiario", "CPF " + beneficiario.CPF + " já existe para o mesmo cliente!");
            }

            if (!this.ModelState.IsValid)
            {
                List<string> erros = (from item in ModelState.Values
                                      from error in item.Errors
                                      select error.ErrorMessage + "<BR>").ToList();

                Response.StatusCode = 400;
                return Json(string.Join(Environment.NewLine, erros));
            }
            else
            {
                bo.Alterar(new Cliente()
                {
                    Id = model.Id,
                    CEP = model.CEP,
                    Cidade = model.Cidade,
                    Email = model.Email,
                    Estado = model.Estado,
                    Logradouro = model.Logradouro,
                    Nacionalidade = model.Nacionalidade,
                    Nome = model.Nome,
                    Sobrenome = model.Sobrenome,
                    Telefone = model.Telefone,
                    CPF = model.CPF,
                    Beneficiarios = ConverterModelParaBeneficiario(model.Beneficiarios)
                });

                return Json("Cadastro alterado com sucesso");
            }
        }

        [HttpGet]
        public ActionResult Alterar(long id)
        {
            BoCliente bo = new BoCliente();
            Cliente cliente = bo.Consultar(id);
            Models.ClienteModel model = null;

            if (cliente != null)
            {
                model = new ClienteModel()
                {
                    Id = cliente.Id,
                    CEP = cliente.CEP,
                    Cidade = cliente.Cidade,
                    Email = cliente.Email,
                    Estado = cliente.Estado,
                    Logradouro = cliente.Logradouro,
                    Nacionalidade = cliente.Nacionalidade,
                    Nome = cliente.Nome,
                    Sobrenome = cliente.Sobrenome,
                    Telefone = cliente.Telefone,
                    CPF = cliente.CPF,
                    Beneficiarios = ConverterBeneficiarioParaModel(cliente.Beneficiarios)
                };
            }

            return View(model);
        }

        [HttpPost]
        public JsonResult ClienteList(int jtStartIndex = 0, int jtPageSize = 0, string jtSorting = null)
        {
            try
            {
                int qtd = 0;
                string campo = string.Empty;
                string crescente = string.Empty;
                string[] array = jtSorting.Split(' ');

                if (array.Length > 0)
                    campo = array[0];

                if (array.Length > 1)
                    crescente = array[1];

                List<Cliente> clientes = new BoCliente().Pesquisa(jtStartIndex, jtPageSize, campo, crescente.Equals("ASC", StringComparison.InvariantCultureIgnoreCase), out qtd);

                //Return result to jTable
                return Json(new { Result = "OK", Records = clientes, TotalRecordCount = qtd });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }

        private static List<BeneficiarioModel> ConverterBeneficiarioParaModel(List<Beneficiario> beneficiarios) {
            List<BeneficiarioModel> retorno = new List<BeneficiarioModel>();
            foreach (var beneficiario in beneficiarios)
            {
                retorno.Add(new BeneficiarioModel()
                {
                    Id = beneficiario.Id,
                    IdCliente = beneficiario.IdCliente,
                    Nome = beneficiario.Nome,
                    CPF = beneficiario.CPF
                });
            }
            return retorno;
        }
        private static List<Beneficiario> ConverterModelParaBeneficiario(List<BeneficiarioModel> beneficiarios)
        {
            List<Beneficiario> retorno = new List<Beneficiario>();
            foreach (var beneficiario in beneficiarios)
            {
                retorno.Add(new Beneficiario()
                {
                    Id = beneficiario.Id,
                    IdCliente = beneficiario.IdCliente,
                    Nome = beneficiario.Nome,
                    CPF = beneficiario.CPF
                });
            }
            return retorno;
        }

        private static bool isCPFValido(string cpf)
        {
            cpf = cpf.Replace(".", "").Replace("-", "");
            if (cpf.Length != 11 || !long.TryParse(cpf, out _))
                return false;

            int[] multiplicador1 = { 10, 9, 8, 7, 6, 5, 4, 3, 2 };
            int[] multiplicador2 = { 11, 10, 9, 8, 7, 6, 5, 4, 3, 2 };

            string tempCpf = cpf.Substring(0, 9);
            int soma = 0;

            for (int i = 0; i < 9; i++)
                soma += int.Parse(tempCpf[i].ToString()) * multiplicador1[i];

            int resto = soma % 11;
            if (resto < 2)
                resto = 0;
            else
                resto = 11 - resto;

            string digito = resto.ToString();
            tempCpf = tempCpf + digito;
            soma = 0;

            for (int i = 0; i < 10; i++)
                soma += int.Parse(tempCpf[i].ToString()) * multiplicador2[i];

            resto = soma % 11;
            if (resto < 2)
                resto = 0;
            else
                resto = 11 - resto;

            digito = digito + resto.ToString();

            return cpf.EndsWith(digito);
        }
    }
}