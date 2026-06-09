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
