# 🏬 API Loja de Estoque

API RESTful desenvolvida com **ASP.NET Core 8**, que gerencia produtos e estoques, com suporte a DTOs, AutoMapper, paginação, logs customizados e tratamento global de exceções.

## 🚀 Tecnologias Utilizadas

- ASP.NET Core 8
- Entity Framework Core
- AutoMapper
- DTOs (Create, Read, Update)
- FluentValidation (se desejar adicionar)
- Swagger / Swashbuckle
- Unit of Work e Repositórios Genéricos
- Paginação assíncrona reutilizável
- Logs customizados em arquivo (`ILogger`)
- Middleware de exceção customizado
- Filtros de ação (`ActionFilter`)

## 📦 Funcionalidades

- CRUD completo de Produtos
- CRUD completo de Estoque
- Inspeção de Estoques por localidade
- Paginação com `pageNumber` e `pageSize`
- Atualizações parciais com `JSON Patch`
- Log em arquivo `.txt` com `ILogger`
- Tratamento global de exceções
- Uso de controllers genéricos com DTOs

## 🛠️ Como Executar

### 🔸 Pré-requisitos

- [.NET SDK 8+](https://dotnet.microsoft.com/download)
- [SQL Server](https://www.microsoft.com/sql-server)
- Visual Studio 2022+ ou VS Code

### 🔹 Clonar o projeto

git clone https://github.com/BrenoRodrigues05/estoque-loja-api.git
cd estoque-loja-api/APILojaEstoque

🔹 Configurar o appsettings.json

"ConnectionStrings": {
  "DefaultConnection": "Server=localhost;Database=LojaEstoqueDb;Trusted_Connection=True;"
}

🔹 Aplicar migrações e rodar

dotnet ef database update
dotnet run

Swagger disponível em: https://localhost:5001/swagger

📄 Exemplo de DTO: EstoqueCreateDTO

{
  "produtoId": 1,
  "local": "Matriz",
  "quantidade": 100,
  "ultimaAtualizacao": "2025-07-30T00:00:00"
}

🧠 Regras de Negócio

1- Não é possível cadastrar estoque sem produto existente

2- Local do estoque é obrigatório (padrão: Matriz)

3- Quantidade de estoque não pode ser negativa

4- Suporte a múltiplos locais por produto

👨‍💻 Autor
Breno Rodrigues
