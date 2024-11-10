CREATE PROC FI_SP_AltBeneficiario
    @Id            BIGINT,
	@IdCliente     BIGINT,
	@NOME          VARCHAR (50) ,
	@CPF		   VARCHAR(14)
AS
BEGIN
	UPDATE BENEFICIARIOS
	SET 
		NOME = @NOME, 
		CPF = @CPF,
		IDCLIENTE = @IdCliente
	WHERE Id = @Id
END