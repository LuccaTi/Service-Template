# Service Template — Serviço Windows em .NET 8 com Clean Architecture

## Visão geral
Este repositório contém um template de Serviço (Worker) construído com .NET 8. O projeto foi estruturado para seguir os princípios de **Clean Architecture**, dividindo suas responsabilidades em camadas bem definidas (Host, Application, Domain, Infrastructure), configuração centralizada e logging estruturado.

O propósito é criar um "esqueleto" escalável e limpo que sirva como base para a criação de novos serviços seguindo as melhores práticas modernas do ecossistema .NET.

A execução do trabalho é feita de forma assíncrona para maximizar a eficiência do uso dos recursos computacionais, ideal para processamento em background.

## Tecnologias e bibliotecas essenciais
- .NET 8 (Worker Service / Console Application)
- Microsoft.Extensions.Hosting (hospedagem, injeção de dependência e ciclo de vida)
- Serilog (logging estruturado)
- Moq e xUnit (para testes de unidade)

## Estrutura do Projeto (Clean Architecture)
O código agora está unificado sob a pasta `src/` e `tests/`, garantindo separação arquitetural e escalabilidade:

- **`src/ServiceTemplate.Domain/`**: Camada central isolada. Contém entidades, interfaces de repositório e regras de negócio essenciais.
- **`src/ServiceTemplate.Application/`**: Orquestração de casos de uso operacionais. 
  - `Orchestrators/`: Define o fluxo principal do trabalhador.
  - `Engines/`: Aplica as regras sistêmicas.
  - *Extensões de Injeção de Dependências (*`DependencyInjection.cs`*)*.
- **`src/ServiceTemplate.Infrastructure/`**: Responsável pelas integrações externas (banco de dados, APIs, mensageria, etc).
- **`src/ServiceTemplate.Host/`**: Ponto de entrada (application root). Configura o `.NET Generic Host`, carregamento do `appsettings.json`, Serilog e faz a ponte (`ServiceLifeCycleManager`) para o orquestrador.
- **`tests/ServiceTemplate.Application.Tests/`**: Testes automatizados das regras e orquestração usando de mocks.

## Arquitetura e Padrões de Projeto
- **Host e Ciclo de Vida**: Utiliza o `.NET Generic Host` (`Microsoft.Extensions.Hosting`), o padrão definitivo e robusto da Microsoft, facilitando a execução via Console, Container ou Serviço do Windows.
- **Injeção de Dependência**: A configuração dos serviços foi segregada. Cada camada possui seu `DependencyInjection.cs`, mantendo o `Program.cs` limpo.
- **Logging (Serilog)**: Logs configurados na raiz do host, garantindo output no Console e arquivo `rolling`, preservando o registro desde o bootstrap.

## Configuração
Arquivo: `src/ServiceTemplate.Host/appsettings.json`

Seções disponíveis:
- **`ServiceSettings`**:
  - `Interval` (int): intervalo em segundos entre execuções do timer.

## Uso e Instalação (Serviço Windows)
Este worker pode rodar em containers, no console ou como Serviço Windows.
Para rodar como serviço Windows, compile a solução na configuração Release e registre via cmd:

1. Abra o cmd **como Administrador**, navegue até a pasta raiz da solução e compile:
   `dotnet build -c Release` (ou utilize `dotnet publish -c Release -o C:\Worker` para gerar uma pasta final de deploy independente).
2. Localize o caminho completo do executável gerado:
   `.../src/ServiceTemplate.Host/bin/Release/net8.0/ServiceTemplate.Host.exe` *(exemplo)*
3. Crie o Serviço via utilitário **SC (Service Control)**:
   Utilize o comando abaixo em qualquer diretório. O parâmetro `binPath` deve conter o caminho exato do executável levantado no passo anterior.
   ```cmd
   sc.exe create "ServiceName" binPath="C:\caminho\completo\ServiceTemplate.Host.exe" DisplayName="Meu Novo Worker"
   ```
4. Iniciar, Parar e Excluir:
   Estes comandos também podem ser executados de qualquer local. O que importa é que o nome do serviço ("ServiceName") seja o mesmo.
   ```cmd
   sc.exe start "ServiceName"
   sc.exe stop "ServiceName"
   sc.exe delete "ServiceName"
   ```

*(Para rodar durante o desenvolvimento, use apenas `dotnet run --project src/ServiceTemplate.Host`).*