# API RocketMovies

Bem-vindo à API RocketMovies, uma API RESTful desenvolvida em ASP.NET Core para gerenciar avaliações de filmes. A API permite que os usuários criem contas, adicionem avaliações e gerenciem informações sobre seus filmes favoritos.

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

💡 **Dica:** Para maior segurança, utilize variáveis de ambiente para armazenar credenciais sensíveis.

### 3️⃣ Aplicando as Migrations

Execute os seguintes comandos para criar o banco de dados:

```bash
dotnet ef database update
```

### 4️⃣ Executando a API

```bash
dotnet run
```

A API estará disponível em:

- `http://localhost:5292`
- `https://localhost:7155`

## 🔑 Autenticação

A API utiliza JWT para autenticação. Após o login, um token será gerado e deverá ser enviado no header das requisições protegidas:

```
Authorization: Bearer SEU_TOKEN_AQUI
```

## 📌 Endpoints

### 🧑🔐 Auth

#### 🔹 Logar Usuário

**POST** `/api/auth/login`

**Descrição:** Autentica um usuário e retorna um token JWT.

### 🧑‍💼 Users

#### 🔹 Criar Usuário

**POST** `/api/users`

**Descrição:** Cria um novo usuário na plataforma.

#### 🔹 Editar Usuário

**PUT** `/api/users`

**Descrição:** Atualiza informações do usuário autenticado.

### 🖼️ Users Avatar

#### 🔹 Atualizar Avatar

**PUT** `/api/users/avatar`

**Descrição:** Atualiza a foto de perfil do usuário.

#### 🔹 Obter Avatar

**GET** `/api/users/avatar/{userId}`

**Descrição:** Obtém a foto de perfil do usuário.

### 🎬 Movies

#### 🔹 Adicionar Filme

**POST** `/api/movies`

**Descrição:** Adiciona um novo filme à biblioteca do usuário.

#### 🔹 Listar Filmes

**GET** `/api/movies`

**Descrição:** Retorna uma lista de filmes cadastrados pelo usuário.

#### 🔹 Obter Detalhes de um Filme

**GET** `/api/movies/{id}`

**Descrição:** Retorna informações detalhadas de um filme.

#### 🔹 Editar Filme

**PUT** `/api/movies/{id}`

**Descrição:** Atualiza informações de um filme.

#### 🔹 Deletar Filme

**DELETE** `/api/movies/{id}`

**Descrição:** Remove um filme da biblioteca do usuário.

## 📜 Licença

Este projeto está sob a licença MIT.

## 🔗 Desenvolvido por Ronaldo Domingues 🚀