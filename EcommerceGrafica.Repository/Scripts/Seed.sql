INSERT INTO public.produtos (id, nome, descricao, preco, moeda, tipo, ativo, criado_em)
VALUES
    ('11111111-1111-1111-1111-111111111111', 'Cartão de Visita 500un', 'Cartão couché 300g', 89.90, 'BRL', 1, TRUE, NOW()),
    ('22222222-2222-2222-2222-222222222222', 'Banner 2x1m', 'Banner em lona com acabamento', 149.90, 'BRL', 2, TRUE, NOW()),
    ('33333333-3333-3333-3333-333333333333', 'Folder A4', 'Folder frente e verso', 199.90, 'BRL', 3, TRUE, NOW())
ON CONFLICT (id) DO NOTHING;
