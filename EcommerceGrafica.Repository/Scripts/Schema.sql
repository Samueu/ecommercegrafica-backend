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
