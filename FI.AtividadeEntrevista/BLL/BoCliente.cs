using FI.AtividadeEntrevista.DAL;
using FI.AtividadeEntrevista.DML;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FI.AtividadeEntrevista.BLL
{
    public class BoCliente
    {
        /// <summary>
        /// Inclui um novo cliente
        /// </summary>
        /// <param name="cliente">Objeto de cliente</param>
        public long Incluir(DML.Cliente cliente)
        {
            DAL.DaoCliente cli = new DAL.DaoCliente();
            DAL.DaoBeneficiario daoBeneficiario = new DAL.DaoBeneficiario();

            long codigoCliente = cli.Incluir(cliente);
            if (cliente.Beneficiarios != null)
            {
                foreach (var beneficiario in cliente.Beneficiarios)
                {
                    beneficiario.IdCliente = codigoCliente;
                    daoBeneficiario.Incluir(beneficiario);
                }
            }

            return codigoCliente;
        }

        /// <summary>
        /// Altera um cliente
        /// </summary>
        /// <param name="cliente">Objeto de cliente</param>
        public void Alterar(DML.Cliente cliente)
        {
            DAL.DaoCliente cli = new DAL.DaoCliente();
            DAL.DaoBeneficiario daoBeneficiario = new DAL.DaoBeneficiario();

            cli.Alterar(cliente);
            List<Beneficiario> beneficiariosAntigos = daoBeneficiario.Listar(cliente.Id);
            List<Beneficiario> beneficiariosExcluir = beneficiariosAntigos.Where(x => !cliente.Beneficiarios.Any(y => y.Id == x.Id)).ToList();

            beneficiariosExcluir.ForEach(x => daoBeneficiario.Excluir(x.Id));
            cliente.Beneficiarios.Where(x => x.Id != 0).ToList().ForEach(x => daoBeneficiario.Alterar(x));
            cliente.Beneficiarios.Where(x => x.Id == 0).ToList().ForEach(x => daoBeneficiario.Incluir(x));
        }

        /// <summary>
        /// Consulta o cliente pelo id
        /// </summary>
        /// <param name="id">id do cliente</param>
        /// <returns></returns>
        public DML.Cliente Consultar(long id)
        {
            DAL.DaoCliente cli = new DAL.DaoCliente();
            DAL.DaoBeneficiario daoBeneficiario = new DAL.DaoBeneficiario();
            Cliente retorno = cli.Consultar(id);

            retorno.Beneficiarios = daoBeneficiario.Listar(id);
            return retorno;
        }

        /// <summary>
        /// Excluir o cliente pelo id
        /// </summary>
        /// <param name="id">id do cliente</param>
        /// <returns></returns>
        public void Excluir(long id)
        {
            DAL.DaoCliente cli = new DAL.DaoCliente();
            cli.Excluir(id);
        }

        /// <summary>
        /// Lista os clientes
        /// </summary>
        public List<DML.Cliente> Listar()
        {
            DAL.DaoCliente cli = new DAL.DaoCliente();
            return cli.Listar();
        }

        /// <summary>
        /// Lista os clientes
        /// </summary>
        public List<DML.Cliente> Pesquisa(int iniciarEm, int quantidade, string campoOrdenacao, bool crescente, out int qtd)
        {
            DAL.DaoCliente cli = new DAL.DaoCliente();
            DAL.DaoBeneficiario daoBeneficiario = new DaoBeneficiario();
            List<DML.Cliente> retorno = cli.Pesquisa(iniciarEm, quantidade, campoOrdenacao, crescente, out qtd);

            retorno.ForEach(x => x.Beneficiarios = daoBeneficiario.Listar(x.Id));

            return retorno;
        }

        /// <summary>
        /// VerificaExistencia
        /// </summary>
        /// <param name="CPF"></param>
        /// <returns></returns>
        public bool VerificarExistencia(string CPF)
        {
            DAL.DaoCliente cli = new DAL.DaoCliente();
            return cli.VerificarExistencia(CPF);
        }
    }
}
