﻿CREATE PROC FI_SP_IncBeneficiario
	@CPF           VARCHAR (11) ,
    @NOME          VARCHAR (50) ,
    @IDCLIENTE     BIGINT		
AS
BEGIN
	INSERT INTO BENEFICIARIOS (NOME, CPF, IDCLIENTE) VALUES (@NOME, @CPF, @IDCLIENTE)
END