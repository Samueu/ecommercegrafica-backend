INSERT INTO public.produtos (nome, descricao, preco, moeda, tipo, ativo, criado_em, imagem_url)
SELECT 'Cartão de Visita 500un', 'Cartão couché 300g', 89.90, 'BRL', 1, TRUE, NOW(),
       'https://placehold.co/600x400?text=Cartao+de+Visita'
WHERE NOT EXISTS (SELECT 1 FROM public.produtos WHERE nome = 'Cartão de Visita 500un');

INSERT INTO public.produtos (nome, descricao, preco, moeda, tipo, ativo, criado_em, imagem_url)
SELECT 'Banner 2x1m', 'Banner em lona com acabamento', 149.90, 'BRL', 2, TRUE, NOW(),
       'https://placehold.co/600x400?text=Banner'
WHERE NOT EXISTS (SELECT 1 FROM public.produtos WHERE nome = 'Banner 2x1m');

INSERT INTO public.produtos (nome, descricao, preco, moeda, tipo, ativo, criado_em, imagem_url)
SELECT 'Folder A4', 'Folder frente e verso', 199.90, 'BRL', 3, TRUE, NOW(),
       'https://placehold.co/600x400?text=Folder'
WHERE NOT EXISTS (SELECT 1 FROM public.produtos WHERE nome = 'Folder A4');

-- ============================================================================
-- Usuários de desenvolvimento (login real via /api/auth/login)
-- Senhas em claro (somente dev): admin@loja.com → Admin@123 | maria.silva@example.com → Cliente@123
-- Hashes BCrypt (work factor 12), compatíveis com PasswordHasher do backend.
-- ============================================================================

INSERT INTO public.clientes (nome, email, telefone, cadastrado_em)
SELECT 'Maria da Silva', 'maria.silva@example.com', '11999990000', NOW()
WHERE NOT EXISTS (SELECT 1 FROM public.clientes WHERE email = 'maria.silva@example.com');

INSERT INTO public.usuarios (email, senha_hash, role, ativo, criado_em, cliente_id, consentimento_em, consentimento_versao)
SELECT 'admin@loja.com',
       '$2a$12$oyqVGfnMkyBFijVIXWyOjOcMoPAdRI.4qUZsohEArMuTRzYJVBlY6',
       2,
       TRUE,
       NOW(),
       NULL,
       NOW(),
       'v1'
WHERE NOT EXISTS (SELECT 1 FROM public.usuarios WHERE email = 'admin@loja.com');

INSERT INTO public.usuarios (email, senha_hash, role, ativo, criado_em, cliente_id, consentimento_em, consentimento_versao)
SELECT 'maria.silva@example.com',
       '$2a$12$i0QM9eTjIj/yCRcj6CQN6empZOCwDCAr3dDjZzg1wnXnCFDwO3t1u',
       1,
       TRUE,
       NOW(),
       (SELECT id FROM public.clientes WHERE email = 'maria.silva@example.com' LIMIT 1),
       NOW(),
       'v1'
WHERE NOT EXISTS (SELECT 1 FROM public.usuarios WHERE email = 'maria.silva@example.com');
