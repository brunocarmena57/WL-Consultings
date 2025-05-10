# Aplicação WLConsultingChallenge

## Visão Geral

Este projeto é uma solução para o Desafio WLConsulting, fornecendo um conjunto de APIs e funcionalidades centrais relacionadas a carteiras de usuários e transações. A aplicação é estruturada em várias camadas para promover a separação de preocupações e a manutenibilidade.

## Estrutura do Projeto

A solução está organizada nos seguintes projetos:

* **WLConsultingChallenge.API:** Este projeto contém os controladores da API Web ASP.NET Core responsáveis por lidar com as requisições HTTP de entrada e retornar respostas. Ele orquestra a lógica da aplicação interagindo com a camada de serviço.
* **WLConsultingChallenge.Core:** Este projeto abriga a lógica de negócios principal e as entidades de domínio (por exemplo, `User`, `Wallet`, `Transaction`). Ele define os conceitos e interfaces fundamentais da aplicação.
* **WLConsultingChallenge.Core.Entities:** Esta pasta dentro do projeto `Core` contém especificamente as definições das entidades da aplicação.
* **WLConsultingChallenge.Core.Services:** Esta pasta dentro do projeto `Core` contém os serviços de lógica de negócios (por exemplo, `UserService`, `WalletService`, `AuthService`). Esses serviços implementam as funcionalidades principais da aplicação, muitas vezes dependendo de abstrações definidas no projeto `Core`.
* **WLConsultingChallenge.Core.Interfaces:** Esta pasta dentro do projeto `Core` define interfaces que abstraem implementações concretas, promovendo o baixo acoplamento e a testabilidade (por exemplo, interfaces de repositório).
* **WLConsultingChallenge.Infra:** Este projeto contém as implementações relacionadas à infraestrutura, como o acesso a dados usando o Entity Framework Core.
* **WLConsultingChallenge.Infra.Data:** Esta pasta dentro do projeto `Infra` abriga o `AppDbContext` (para interação com o banco de dados) e potencialmente implementações concretas de repositório.
* **WLConsultingChallenge.Infra.Data.Repositories:** Esta pasta dentro do projeto `Infra` contém as implementações concretas das interfaces de repositório definidas no projeto `Core`.
* **WLConsultingChallenge.DTos:** Este projeto contém Objetos de Transferência de Dados usados para comunicação entre diferentes camadas ou para corpos de requisição/resposta da API.

## Tecnologias Utilizadas

* ASP.NET Core
* Entity Framework Core
* C#

## Como Começar

### Pré-requisitos

* [.NET SDK](https://dotnet.microsoft.com/download) instalado em sua máquina.
* SQL Server ou outro banco de dados compatível configurado (a string de conexão precisa ser definida em `appsettings.json` do projeto da API).

### Instalação

1.  **Clone o repositório:**
    ```bash
    git clone [https://github.com/brunocarmena57/WL-Consulting](https://github.com/brunocarmena57/WL-Consulting))
    cd WLConsultingChallenge
    ```

2.  **Navegue até o projeto da API:**
    ```bash
    cd WLConsultingChallenge.API
    ```

3.  **Atualize a String de Conexão do Banco de Dados:**
    * Abra o arquivo `appsettings.json`.
    * Localize a seção `ConnectionStrings`.
    * Atualize o valor de `DefaultConnection` com sua string de conexão do SQL Server.

4.  **Aplique as Migrações do Banco de Dados:**
    ```bash
    dotnet ef database update -p ../WLConsultingChallenge.Infra -s .
    ```
    Este comando criará o banco de dados e aplicará quaisquer migrações pendentes definidas no projeto `WLConsultingChallenge.Infra`.

5.  **Construa a solução:**
    ```bash
    dotnet build
    ```

### Executando a Aplicação

1.  **Navegue até o diretório do projeto da API:**
    ```bash
    cd WLConsultingChallenge.API
    ```

2.  **Execute a aplicação:**
    ```bash
    dotnet run
    ```

A API normalmente será iniciada em uma porta padrão. Você pode encontrar a porta exata na saída do console.

## Endpoints da API

A API fornece os seguintes endpoints para interagir com funcionalidades de carteira:

* **`GET /api/wallet/balance`**: Recupera o saldo da carteira do usuário autenticado. Requer autenticação.
    * **Resposta (Sucesso - 200 OK):**
      ```json
      {
        "balance": 100.50
      }
      ```
    * **Resposta (Erro - 400 Bad Request):**
      ```json
      {
        "message": "Mensagem de erro."
      }
      ```

* **`POST /api/wallet/deposit`**: Deposita um valor na carteira do usuário autenticado. Requer autenticação.
    * **Corpo da Requisição (application/json):**
      ```json
      {
        "amount": 50.00,
        "description": "Depósito inicial"
      }
      ```
    * **Resposta (Sucesso - 200 OK):**
      ```json
      {
        "message": "Depósito realizado com sucesso",
        "newBalance": 150.50
      }
      ```
    * **Resposta (Erro - 400 Bad Request):**
      ```json
      {
        "message": "Mensagem de erro."
      }
      ```

* **`POST /api/wallet/transfer`**: Transfere um valor da carteira do usuário autenticado para outra carteira de usuário. Requer autenticação.
    * **Corpo da Requisição (application/json):**
      ```json
      {
        "toUserId": 2,
        "amount": 25.00,
        "description": "Transferência para o usuário 2"
      }
      ```
    * **Resposta (Sucesso - 200 OK):**
      ```json
      {
        "message": "Transferência realizada com sucesso",
        "newBalance": 125.50
      }
      ```
    * **Resposta (Erro - 400 Bad Request):**
      ```json
      {
        "message": "Mensagem de erro."
      }
      ```

* **`GET /api/wallet/transactions`**: Recupera o histórico de transações do usuário autenticado, com opção de filtrar por período. Requer autenticação.
    * **Parâmetros de Query (opcionais):**
        * `startDate`: Data de início para filtrar as transações (formato YYYY-MM-DD).
        * `endDate`: Data de fim para filtrar as transações (formato YYYY-MM-DD).
    * **Resposta (Sucesso - 200 OK):**
      ```json
      [
        {
          "id": 1,
          "fromUsername": null,
          "toUsername": "usuario_autenticado",
          "amount": 50.00,
          "type": "Deposit",
          "createdAt": "2025-05-08T15:30:00Z",
          "description": "Depósito inicial"
        },
        {
          "id": 2,
          "fromUsername": "usuario_autenticado",
          "toUsername": "outro_usuario",
          "amount": 25.00,
          "type": "Transfer",
          "createdAt": "2025-05-08T15:35:00Z",
          "description": "Transferência para outro usuário"
        }
        // ... outras transações
      ]
      ```
    * **Resposta (Erro - 400 Bad Request):**
      ```json
      {
        "message": "Mensagem de erro."
      }
      ```

**Observação:** Todos os endpoints listados em `/api/wallet/*` requerem autenticação de usuário.

## Contribuindo

1.  Faça um fork do repositório.
2.  Crie uma nova branch para sua feature ou correção de bug.
3.  Faça suas alterações e commite-as.
4.  Faça push para o seu fork.
5.  Envie um pull request.
