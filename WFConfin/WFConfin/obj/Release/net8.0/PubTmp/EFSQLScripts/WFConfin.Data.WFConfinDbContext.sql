CREATE TABLE IF NOT EXISTS "__EFMigrationsHistory" (
    "MigrationId" character varying(150) NOT NULL,
    "ProductVersion" character varying(32) NOT NULL,
    CONSTRAINT "PK___EFMigrationsHistory" PRIMARY KEY ("MigrationId")
);

START TRANSACTION;


DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20240706002042_Initial') THEN
    CREATE TABLE "Conta" (
        "Id" uuid NOT NULL,
        "Descricao" character varying(200) NOT NULL,
        "Valor" numeric(18,2) NOT NULL,
        "DataVencimento" timestamp with time zone NOT NULL,
        "DataPagamento" timestamp with time zone,
        "Situacao" integer NOT NULL,
        "PessoaId" uuid NOT NULL,
        CONSTRAINT "PK_Conta" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20240706002042_Initial') THEN
    CREATE TABLE "Estado" (
        "Sigla" character varying(2) NOT NULL,
        "Nome" character varying(60) NOT NULL,
        CONSTRAINT "PK_Estado" PRIMARY KEY ("Sigla")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20240706002042_Initial') THEN
    CREATE TABLE "Usuario" (
        "Id" uuid NOT NULL,
        "Nome" character varying(200) NOT NULL,
        "Login" character varying(20) NOT NULL,
        "Password" character varying(200) NOT NULL,
        "Funcao" character varying(20) NOT NULL,
        CONSTRAINT "PK_Usuario" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20240706002042_Initial') THEN
    CREATE TABLE "Cidade" (
        "Id" uuid NOT NULL,
        "Nome" character varying(200) NOT NULL,
        "EstadoSigla" character varying(2) NOT NULL,
        CONSTRAINT "PK_Cidade" PRIMARY KEY ("Id"),
        CONSTRAINT "FK_Cidade_Estado_EstadoSigla" FOREIGN KEY ("EstadoSigla") REFERENCES "Estado" ("Sigla") ON DELETE CASCADE
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20240706002042_Initial') THEN
    CREATE TABLE "Pessoa" (
        "Id" uuid NOT NULL,
        "Nome" character varying(200) NOT NULL,
        "Telefone" character varying(20) NOT NULL,
        "Email" text,
        "DataNascimento" timestamp with time zone,
        "Salario" numeric(18,2) NOT NULL,
        "Genero" character varying(20),
        "CidadeId" uuid,
        CONSTRAINT "PK_Pessoa" PRIMARY KEY ("Id"),
        CONSTRAINT "FK_Pessoa_Cidade_CidadeId" FOREIGN KEY ("CidadeId") REFERENCES "Cidade" ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20240706002042_Initial') THEN
    CREATE INDEX "IX_Cidade_EstadoSigla" ON "Cidade" ("EstadoSigla");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20240706002042_Initial') THEN
    CREATE INDEX "IX_Pessoa_CidadeId" ON "Pessoa" ("CidadeId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20240706002042_Initial') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20240706002042_Initial', '8.0.6');
    END IF;
END $EF$;
COMMIT;

