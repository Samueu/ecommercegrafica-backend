CREATE TABLE IF NOT EXISTS public.produtos (
    id          SERIAL4         PRIMARY KEY,
    nome        VARCHAR(200)    NOT NULL,
    descricao   VARCHAR(1000)   NOT NULL DEFAULT '',
    preco       NUMERIC(18, 2)  NOT NULL,
    moeda       VARCHAR(3)      NOT NULL DEFAULT 'BRL',
    tipo        INTEGER         NOT NULL,
    ativo       BOOLEAN         NOT NULL DEFAULT TRUE,
    criado_em   TIMESTAMPTZ     NOT NULL,
    imagem_url  VARCHAR(500)    NULL
);

ALTER TABLE public.produtos ADD COLUMN IF NOT EXISTS imagem_url VARCHAR(500) NULL;

CREATE TABLE IF NOT EXISTS public.clientes (
    id              SERIAL4         PRIMARY KEY,
    nome            VARCHAR(200)    NOT NULL,
    email           VARCHAR(256)    NOT NULL UNIQUE,
    telefone        VARCHAR(20)     NULL,
    cadastrado_em   TIMESTAMPTZ     NOT NULL
);

CREATE TABLE IF NOT EXISTS public.pedidos (
    id          SERIAL4         PRIMARY KEY,
    cliente_id  INTEGER         NOT NULL REFERENCES public.clientes(id),
    status      INTEGER         NOT NULL,
    criado_em   TIMESTAMPTZ     NOT NULL,
    logradouro  VARCHAR(200)    NULL,
    numero      VARCHAR(20)     NULL,
    bairro      VARCHAR(100)    NULL,
    cidade      VARCHAR(100)    NULL,
    estado      VARCHAR(2)      NULL,
    cep         VARCHAR(10)     NULL
);

CREATE INDEX IF NOT EXISTS ix_pedidos_cliente_id ON public.pedidos(cliente_id);

CREATE TABLE IF NOT EXISTS public.itens_pedido (
    id              SERIAL4         PRIMARY KEY,
    pedido_id       INTEGER         NOT NULL REFERENCES public.pedidos(id) ON DELETE CASCADE,
    produto_id      INTEGER         NOT NULL REFERENCES public.produtos(id),
    nome_produto    VARCHAR(200)    NOT NULL,
    quantidade      INTEGER         NOT NULL,
    preco_unitario  NUMERIC(18, 2)  NOT NULL
);

CREATE INDEX IF NOT EXISTS ix_itens_pedido_pedido_id ON public.itens_pedido(pedido_id);

-- ============================================================================
-- Autenticação / LGPD
-- ============================================================================

CREATE TABLE IF NOT EXISTS public.usuarios (
    id                     SERIAL4         PRIMARY KEY,
    email                  VARCHAR(256)    NOT NULL UNIQUE,
    senha_hash             VARCHAR(255)    NOT NULL,
    role                   INTEGER         NOT NULL DEFAULT 1,
    ativo                  BOOLEAN         NOT NULL DEFAULT TRUE,
    criado_em              TIMESTAMPTZ     NOT NULL,
    ultimo_login_em        TIMESTAMPTZ     NULL,
    cliente_id             INTEGER         NULL REFERENCES public.clientes(id) ON DELETE SET NULL,
    consentimento_em       TIMESTAMPTZ     NULL,
    consentimento_versao   VARCHAR(20)     NULL
);

CREATE INDEX IF NOT EXISTS ix_usuarios_cliente_id ON public.usuarios(cliente_id);

CREATE TABLE IF NOT EXISTS public.refresh_tokens (
    id                 BIGSERIAL       PRIMARY KEY,
    usuario_id         INTEGER         NOT NULL REFERENCES public.usuarios(id) ON DELETE CASCADE,
    token_hash         VARCHAR(128)    NOT NULL UNIQUE,
    criado_em          TIMESTAMPTZ     NOT NULL,
    expira_em          TIMESTAMPTZ     NOT NULL,
    revogado_em        TIMESTAMPTZ     NULL,
    motivo_revogacao   VARCHAR(100)    NULL,
    ip_origem          VARCHAR(64)     NULL,
    user_agent         VARCHAR(400)    NULL,
    substituido_por_id BIGINT          NULL REFERENCES public.refresh_tokens(id) ON DELETE SET NULL
);

CREATE INDEX IF NOT EXISTS ix_refresh_tokens_usuario_id ON public.refresh_tokens(usuario_id);

CREATE TABLE IF NOT EXISTS public.auth_audit (
    id                 BIGSERIAL       PRIMARY KEY,
    usuario_id         INTEGER         NULL REFERENCES public.usuarios(id) ON DELETE SET NULL,
    email_tentativa    VARCHAR(256)    NOT NULL DEFAULT '',
    evento             VARCHAR(40)     NOT NULL,
    sucesso            BOOLEAN         NOT NULL,
    ip_origem          VARCHAR(64)     NULL,
    user_agent         VARCHAR(400)    NULL,
    detalhe            VARCHAR(500)    NULL,
    criado_em          TIMESTAMPTZ     NOT NULL
);

CREATE INDEX IF NOT EXISTS ix_auth_audit_email_evento ON public.auth_audit(email_tentativa, evento);
CREATE INDEX IF NOT EXISTS ix_auth_audit_criado_em ON public.auth_audit(criado_em);
