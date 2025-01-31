Sistema de Gerenciamento de Eventos

Descrição

Este sistema de gerenciamento de eventos permite a criação, edição e remoção de usuários e eventos, bem como a vinculação e remoção de usuários a esses eventos. O sistema também possui uma tela de login com autenticação JWT para controle de acesso.

Tecnologias Utilizadas

Back-end: .NET Core 6
Front-end: Angular 17
ORM: Entity Framework (Code First)
Autenticação: JWT
Banco de Dados: SQL Server

Arquitetura: Segregação por responsabilidades:
Application: Contém a regra de negócio
Domain: Modelos e entidades
WebAPI: Endpoints da aplicação
Infrastructure: Persistência no banco de dados e configuração das tabelas

Funcionalidades

CRUD de usuários
CRUD de eventos
Inscrição do usuário no evento selecionado
Desinscrição do usuário no evento selecionado
Autenticação via JWT

Páginas:
Home
Usuário
Login
Eventos

Débitos Técnicos e Melhorias

Durante o desenvolvimento, ficaram alguns débitos técnicos e oportunidades de melhorias, especialmente na integração entre o back-end e o front-end. A comunicação entre os dois pode ser otimizada para melhorar a experiência do usuário e a eficiência do sistema.

No front-end, poderiam ter sido implementados padrões de projeto mais consistentes para tornar o sistema mais escalável e sustentável a longo prazo. Além disso, ajustes e melhorias ainda precisam ser realizados para garantir maior qualidade e manutenibilidade do código.
