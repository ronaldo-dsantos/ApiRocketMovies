# API RocketMovies

Bem-vindo à API RocketMovies, uma API RESTful desenvolvida em ASP.NET Core para gerenciar avaliações de filmes. A API permite que os usuários criem contas, adicionem avaliações e gerenciem informações sobre seus filmes favoritos.

## 📌 Tecnologias Utilizadas

- ASP.NET Core
- Entity Framework Core
- SQL Server
- ASP.NET Identity (para autenticação e autorização)
- LINQ
- JWT (JSON Web Token)

## 📖 Sumário

- 🚀 [Instalação e Configuração](#-instalação-e-configuração)
- 🔑 [Autenticação](#-autenticação)
- 📌 [Endpoints](#-endpoints)
- 🛠 [Contribuição](#-contribuição)
- 📜 [Licença](#-licença)

## 🚀 Instalação e Configuração

### 1️⃣ Clonando o repositório

```bash
git clone https://github.com/ronaldo-dsantos/ApiRocketMovies.git
cd ApiRocketMovies
```

### 2️⃣ Configurando o banco de dados

A API utiliza SQL Server. Configure a connection string no arquivo `appsettings.json` ou utilize variáveis de ambiente para armazenar credenciais sensíveis:

```json
"ConnectionStrings": {
    "DefaultConnection": "Server=SEU_SERVIDOR;Database=RocketMoviesDB;User Id=SEU_USUARIO;Password=SUA_SENHA;"
}
```

### 3️⃣ Aplicando as Migrations

Execute os seguintes comandos para criar o banco de dados:

```bash
dotnet ef database update
```

### 4️⃣ Executando a API

```bash
dotnet run
```

A API estará disponível em `http://localhost:5292` ou `https://localhost:7155`.

## 🔑 Autenticação

A API usa JWT para autenticação. Após o login, um token será gerado e deve ser enviado no header das requisições protegidas:

```
Authorization: Bearer SEU_TOKEN_AQUI
```

## 📌 Endpoints

### 🔒 Autenticação

| Método | Rota                   | Descrição                  |
|--------|------------------------|----------------------------|
| POST   | /api/auth              | Autentica o usuário        |

### 🧑 Usuários

| Método | Rota                   | Descrição                  |
|--------|------------------------|----------------------------|
| POST   | /api/users             | Cria um novo usuário       |
| PUT    | /api/users             | Edita um usuário           |

### 🖼️ Avatar

| Método | Rota                   | Descrição                  |
|--------|------------------------|----------------------------|
| PATCH  | /api/avatar            | Edita o avatar do usuário  |
| GET    | /api/avatar/{filename} | Obtém o avatar do usuário  |

### 🎬 Filmes

| Método | Rota                   | Descrição                  |
|--------|------------------------|----------------------------|
| POST   | /api/movies            | Adiciona um novo filme     |
| GET    | /api/movies            | Lista todos os filmes      |
| GET    | /api/movies/{id}       | Obtém detalhes de um filme |
| PUT    | /api/movies/{id}       | Edita um filme             |
| DELETE | /api/movies/{id}       | Remove um filme            |

## 🛠 Contribuição

Contribuições são bem-vindas! Siga os passos abaixo para colaborar:

1. Fork o repositório.
2. Crie um branch para sua feature:

    ```bash
    git checkout -b minha-feature
    ```

3. Commit suas alterações:

    ```bash
    git commit -m "Minha nova feature"
    ```

4. Envie suas alterações:

    ```bash
    git push origin minha-feature
    ```

5. Abra um Pull Request.

## 📜 Licença

Este projeto está sob a licença MIT.

🔗 Desenvolvido por Ronaldo Domingues 🚀