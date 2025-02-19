# API RocketMovies

Bem-vindo à API RocketMovies, uma API RESTful desenvolvida em ASP.NET para gerenciar avaliações de filmes. A API permite que os usuários criem contas, adicionem avaliações, editem informações e consultem dados sobre filmes.

## 📌 Tecnologias Utilizadas

- ASP.NET Core
- Entity Framework Core
- SQL Server
- ASP.NET Identity (para autenticação e autorização)
- LINQ
- JWT (JSON Web Token)

## 🚀 Instalação e Configuração

### 1️⃣ Clonando o repositório

```bash
git clone https://github.com/ronaldo-dsantos/ApiRocketMovies.git
cd ApiRocketMovies
```

### 2️⃣ Configurando o banco de dados

A API utiliza SQL Server. Configure a connection string no arquivo `appsettings.json`:

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

### 🧑‍💼 Auth
#### 🔹 Login

### 🧑‍💼 Users
#### 🔹 CreateUser
#### 🔹 UpdateUser

### 🧑‍💼 UsersAvatar
#### 🔹 UpdateAvatar
#### 🔹 GetAvatar

### 🎬 Movies
#### 🔹 GetMoviesAll
#### 🔹 GetMovieById
#### 🔹 CreateMovie
#### 🔹 UpdateMovie
#### 🔹 DeleteMovie

## 📜 Licença

Este projeto está sob a licença MIT.

🔗 Desenvolvido por Ronaldo Domingues 🚀