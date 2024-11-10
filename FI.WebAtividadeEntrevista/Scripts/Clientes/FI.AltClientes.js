
$(document).ready(function () {
    if (obj) {
        $('#formCadastro #Nome').val(obj.Nome);
        $('#formCadastro #CEP').val(obj.CEP);
        $('#formCadastro #Email').val(obj.Email);
        $('#formCadastro #Sobrenome').val(obj.Sobrenome);
        $('#formCadastro #Nacionalidade').val(obj.Nacionalidade);
        $('#formCadastro #Estado').val(obj.Estado);
        $('#formCadastro #Cidade').val(obj.Cidade);
        $('#formCadastro #Logradouro').val(obj.Logradouro);
        $('#formCadastro #Telefone').val(obj.Telefone);
        $('#formCadastro #CPF').val(obj.CPF);
    }

    $('#formCadastro').submit(function (e) {
        e.preventDefault();
        console.log(obj.Beneficiarios);
        $.ajax({
            url: urlPost,
            method: "POST",
            data: {
                "NOME": $(this).find("#Nome").val(),
                "CEP": $(this).find("#CEP").val(),
                "Email": $(this).find("#Email").val(),
                "Sobrenome": $(this).find("#Sobrenome").val(),
                "Nacionalidade": $(this).find("#Nacionalidade").val(),
                "Estado": $(this).find("#Estado").val(),
                "Cidade": $(this).find("#Cidade").val(),
                "Logradouro": $(this).find("#Logradouro").val(),
                "Telefone": $(this).find("#Telefone").val(),
                "CPF": $(this).find("#CPF").val(),
                "Beneficiarios": obj.Beneficiarios
            },
            error:
            function (r) {
                if (r.status == 400)
                    ModalDialog("Ocorreu um erro", r.responseJSON);
                else if (r.status == 500)
                    ModalDialog("Ocorreu um erro", "Ocorreu um erro interno no servidor.");
            },
            success:
            function (r) {
                ModalDialog("Sucesso!", r)
                $("#formCadastro")[0].reset();                                
                window.location.href = urlRetorno;
            }
        });
    })

    if (document.getElementById("gridBeneficiarios"))
        $('#gridBeneficiarios').jtable({
            title: 'Beneficiarios',
            paging: false, //Enable paging
            pageSize: 5, //Set page size (default: 10)
            sorting: false, //Enable sorting
            defaultSorting: 'Nome ASC', //Set default sorting
            actions: {
                listAction: function (postData, jtParams) {
                    return {
                        "Result": "OK",
                        "Records": obj.Beneficiarios
                    };
                }
            },
            fields: {
                CPF: {
                    title: 'CPF',
                    width: '20px'
                },
                Nome: {
                    title: 'Nome',
                    width: '30px'
                },
                Acoes: {
                    title: 'Ações',
                    width: '50px',
                    display: function (data) {
                        return '<button onclick="AlterarBeneficiario(' + data.record.Id + ');" class="btn btn-primary">Alterar</button>' +
                            '<button onclick="DeletarBeneficiario(' + data.record.Id + ');" class="btn btn-primary">Excluir</button>';
                    }
                }
            }
        });

    //Load student list from server
    if (document.getElementById("gridBeneficiarios"))
        $('#gridBeneficiarios').jtable('load');

})
function ModalDialogBeneficiarios() {
    $('#modalBeneficiario').modal('show');
}

function GravarBeneficiario() {
    let idBeneficiario = $("#IdBeneficiario").val();

    if (idBeneficiario == '' || idBeneficiario == null) {
        obj.Beneficiarios.push({
            "Id": 0,
            "IdCliente": obj.Id,
            "Nome": $("#NomeBeneficiario").val(),
            "CPF": $("#CPFBeneficiario").val()
        })
    } else {
        let index = obj.Beneficiarios.findIndex(x => x.Id == idBeneficiario);
        obj.Beneficiarios[index].Nome = $("#NomeBeneficiario").val();
        obj.Beneficiarios[index].CPF = $("#CPFBeneficiario").val();
    }
    console.log(obj.Beneficiarios);
    $('#IdBeneficiario').val('');
    $('#NomeBeneficiario').val('');
    $('#CPFBeneficiario').val('');
    $('#gridBeneficiarios').jtable('load');
}

function AlterarBeneficiario(idBeneficiario) {
    let beneficiario = obj.Beneficiarios.find(x => x.Id == idBeneficiario)
    console.log(beneficiario);
    $('#IdBeneficiario').val(beneficiario.Id);
    $('#NomeBeneficiario').val(beneficiario.Nome);
    $('#CPFBeneficiario').val(beneficiario.CPF);
}

function DeletarBeneficiario(idBeneficiario) {
    obj.Beneficiarios = obj.Beneficiarios.filter(x => x.Id != idBeneficiario);
    $('#gridBeneficiarios').jtable('load');
}

function ModalDialog(titulo, texto) {
    var random = Math.random().toString().replace('.', '');
    var texto = '<div id="' + random + '" class="modal fade">                                                               ' +
        '        <div class="modal-dialog">                                                                                 ' +
        '            <div class="modal-content">                                                                            ' +
        '                <div class="modal-header">                                                                         ' +
        '                    <button type="button" class="close" data-dismiss="modal" aria-hidden="true">×</button>         ' +
        '                    <h4 class="modal-title">' + titulo + '</h4>                                                    ' +
        '                </div>                                                                                             ' +
        '                <div class="modal-body">                                                                           ' +
        '                    <p>' + texto + '</p>                                                                           ' +
        '                </div>                                                                                             ' +
        '                <div class="modal-footer">                                                                         ' +
        '                    <button type="button" class="btn btn-default" data-dismiss="modal">Fechar</button>             ' +
        '                                                                                                                   ' +
        '                </div>                                                                                             ' +
        '            </div><!-- /.modal-content -->                                                                         ' +
        '  </div><!-- /.modal-dialog -->                                                                                    ' +
        '</div> <!-- /.modal -->                                                                                        ';

    $('body').append(texto);
    $('#' + random).modal('show');
}
