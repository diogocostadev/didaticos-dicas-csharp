Com certeza! Aqui estão todas as 100 dicas de C# mencionadas nas fontes, organizadas em uma lista:

*   **Dica 1: Retornando Coleções Vazias**
    *   Ao lidar com coleções como `arrays` ou `lists` em C#, retornar um `array` ou `list` vazio (`new T` ou `new List<T>()`) aloca memória no *heap* toda vez que é invocado. Isso pode levar a pausas no aplicativo devido à coleta de lixo.
    *   A solução é usar `Array.Empty<T>()` para *arrays* e `Enumerable.Empty<T>()` para outros `IEnumerables`. Isso garante que a coleção vazia seja alocada apenas uma vez durante a vida útil do aplicativo.

*   **Dica 2: Relançando Exceções Corretamente**
    *   Relançar uma exceção usando `throw exceptionVariable;` ignora completamente o *stack trace* da exceção, que contém informações úteis.
    *   Para relançar a exceção exata que foi capturada, incluindo o *stack trace* completo, use apenas `throw;`.

*   **Dica 3: Travamento (Locking) com Async/Await**
    *   A palavra-chave `lock` em C# permite envolver um bloco de código, garantindo que apenas uma *thread* possa acessá-lo por vez, o que é útil em aplicações multi-threaded.
    *   No entanto, você não pode usar `lock` com `async` ou `await`.
    *   A alternativa é usar a classe `SemaphoreSlim`. Crie um objeto `SemaphoreSlim` com o valor 1, use `await semaphore.WaitAsync()` para iniciar o travamento e `semaphore.Release()` para liberá-lo. É uma boa prática chamar `Release()` em um bloco `finally` para garantir que seja sempre executado, prevenindo *deadlocks*.

*   **Dica 4: Armadilhas de Desempenho do LINQ**
    *   O LINQ pode ter uma armadilha de desempenho não tão óbvia: a enumeração múltipla. Se você chamar métodos como `Count()` e `All()` separadamente em um `IEnumerable`, ele será enumerado e construído várias vezes, o que pode impactar severamente o desempenho e até causar múltiplas chamadas de E/S em bancos de dados.
    *   Para corrigir, enumere o `IEnumerable` em uma estrutura apropriada, como uma `List` ou um `Array`, e depois opere sobre ela.

*   **Dica 5: C# REPL (Crebel)**
    *   Para testar rapidamente um pedaço de código C# sem abrir o IDE, use a ferramenta de linha de comando *cross-platform* `crebel`.
    *   Instale-o globalmente com `dotnet tool install -g crebel` e use `crebel` para entrar no modo C#. Ele oferece suporte a *IntelliSense*, auto-completar, sugestões e até permite instalar pacotes NuGet e rodar APIs ASP.NET Core.

*   **Dica 6: Acessando Span de uma Lista (List)**
    *   Enquanto `Span<T>` é ideal para trabalhar com *arrays* rapidamente, `List<T>` é o tipo de coleção mais comum. Cada `List` é internamente suportada por um *array*.
    *   Você pode usar a classe `CollectionsMarshal` e seu método `AsSpan()` para obter acesso a esse *array* interno de uma `List`.
    *   **Cuidado:** isso é uma operação insegura. Se a `List` for mutada enquanto você itera sobre o `Span`, você não receberá uma exceção como normalmente aconteceria.

*   **Dica 7: Logging Correto no .NET**
    *   O *logger* embutido no .NET se refere à parte de texto do *logging* como "mensagem", mas na verdade é um "modelo de mensagem" (*message template*).
    *   Usar interpolação de *string*, formatação de *string* ou concatenação de *string* com seu *logger* está incorreto. Isso faz com que você perca todos os parâmetros do método de *logging* (impossibilitando a filtragem) e desperdiça memória com *strings* que precisam ser coletadas pelo coletor de lixo.
    *   Simplesmente nomeie seus parâmetros com um nome descritivo como parte do seu modelo de mensagem e forneça seus argumentos como um segundo parâmetro. Isso previne problemas de memória e facilita a filtragem.

*   **Dica 8: Tipos Vazios (Empty Types) no C# 12**
    *   C# 12 introduziu uma "característica secreta" para definir tipos vazios (como `structs`, `classes` ou `interfaces`).
    *   Agora você pode omitir as chaves (`{}`) e adicionar um ponto e vírgula (`;`) ao final da declaração, resultando em um código mais limpo para tipos vazios.

*   **Dica 9: ToList() vs ToArray()**
    *   **Funcionalidade:** Use `ToList()` se o consumidor for adicionar ou remover itens. Use `ToArray()` se o consumidor for apenas enumerar ou mutar valores existentes sem alterar o comprimento.
    *   **Desempenho:** `ToList()` é ligeiramente mais rápido que `ToArray()` para coleções maiores (ex: 10.000 itens), o que é "estranho". No entanto, o .NET 9 traz uma atualização de desempenho que torna `ToArray()` muito mais rápido que `ToList()`.

*   **Dica 10: Marcadores de Assembly para Injeção de Dependência**
    *   Ao registrar bibliotecas como *MediatR* ou *AutoMapper* na Injeção de Dependência que aceitam um tipo genérico como marcador de assembly, as pessoas geralmente usam o arquivo `Program.cs`.
    *   Uma abordagem melhor é criar uma **interface vazia nomeada após o assembly** em que ela reside e usá-la como marcador. Isso remove a ambiguidade e torna o código mais legível.

*   **Dica 11: Atributo StringSyntax para Destaque de Texto**
    *   Antes do .NET 7, *strings* que representavam expressões regulares, URLs ou JSON eram tratadas como *strings* simples no IDE.
    *   O .NET 7 introduziu o atributo `StringSyntax`. Ao aplicá-lo a um parâmetro de método e especificar o que o texto representa (ex: `"Regex"`, `"Uri"`, `"Json"`), o IDE destacará o texto apropriadamente, melhorando a experiência do desenvolvedor.

*   **Dica 12: Problemas com Construtores Primários e Campos Readonly**
    *   Construtores primários no C# 12 permitem simplificar classes.
    *   No entanto, atualmente **não há suporte para definir campos `readonly` através de um construtor primário**. Se você injetar um serviço que deve ser imutável com `readonly`, não pode fazê-lo diretamente com construtores primários. IDEs podem sugerir isso como um refatoramento, mas resultará em um código fundamentalmente pior.

*   **Dica 13: UUID v7 (GUID v7) no .NET 9**
    *   GUIDs existentes (UUID v4 com "Microsoft spin") são projetados para serem o mais aleatórios possível, o que pode causar fragmentação em alguns bancos de dados quando usados como chaves.
    *   O .NET 9 introduz um novo tipo de ID, `UUID v7` (também conhecido como `Guid v7` ou `ULID`), que resolve esse problema. Ele substitui os primeiros *bytes* do GUID por dados relacionados ao tempo, tornando-o **ordenável**.
    *   Isso previne a fragmentação e elimina a necessidade de bibliotecas de terceiros para GUIDs ordenáveis. Você pode criá-los usando `Guid.CreateVersion7()`.

*   **Dica 14: O Menor Programa C# Válido**
    *   Desde a introdução de *top-level statements* no C# 9, o método `public static void Main(string[] args)` não é mais necessário no `Program.cs`.
    *   O menor código C# válido em `Program.cs` é simplesmente `;` (um ponto e vírgula).

*   **Dica 15: Cancellation Tokens em APIs ASP.NET Core**
    *   Em APIs ASP.NET Core, não crie seus próprios `CancellationToken`.
    *   Em vez disso, adicione um parâmetro `CancellationToken` ao seu *endpoint* de API (ação do controlador ou método de API mínima). O *framework* ASP.NET Core fornecerá um *token* específico para sua requisição.
    *   Se o usuário cancelar a requisição, qualquer processo em cascata também será cancelado. **Certifique-se de passar o `CancellationToken` para todas as operações subsequentes**.

*   **Dica 16: Inicializadores de Coleção em C# 12**
    *   C# 12 introduziu novos inicializadores de coleção usando apenas dois colchetes (`[]`).
    *   Isso simplifica a criação de coleções como *arrays*, *lists*, *dictionaries* e tipos imutáveis, resultando em um código mais limpo.

*   **Dica 17: Verificando Pacotes NuGet Desatualizados**
    *   Você não precisa do gerenciador de pacotes do IDE para ver quais pacotes NuGet estão desatualizados.
    *   Use a ferramenta CLI `dotnet outdated`. Instale-a globalmente (`dotnet tool install -g dotnet-outdated`) e execute `dotnet outdated`.
    *   A ferramenta mostra pacotes desatualizados e permite atualizá-los com a opção `--upgrade`. Ela também suporta travamento de versão.

*   **Dica 18: Geração de Texto Waffle (Waffle Generation)**
    *   `Lorem Ipsum` não é realista para geração de texto falso.
    *   Desenvolvedores experientes usam "geração de waffle" (`waffle generation`) para gerar texto personalizável e realista.
    *   No .NET, instale o pacote NuGet `WaffleGenerator`. Use a classe `WaffleEngine` com métodos como `HtmlText()` ou `Markdown()`.
    *   Com a integração `Bogus`, você pode definir `waffle` para qualquer propriedade do seu tipo.

*   **Dica 19: Métodos WebApplication (Run, Use, Map)**
    *   A classe `WebApplication` no .NET tem métodos importantes: `Run`, `Use` e `Map`.
    *   `Run`: Adiciona um *middleware* de pipeline terminante.
    *   `Use`: Adiciona um *middleware* geral no pipeline.
    *   `Map`: Mapeia um *middleware* para um caminho de requisição específico.
    *   A **ordem** em que esses métodos são chamados é muito importante.

*   **Dica 20: Validando Naughty Strings**
    *   A maioria das pessoas não valida para um tipo importante de entrada do usuário: `naughty strings`. São *strings* que podem causar *crash* no servidor ou expor vulnerabilidades de segurança.
    *   Instale o pacote NuGet `NaughtyStrings` e use a classe `NaughtyStrings` para recuperar vários tipos de *naughty strings*.
    *   Valide-as em testes de QA e *end-to-end*.

*   **Dica 21: Interpolated Parser (Análise de String Reversa)**
    *   Para analisar valores de uma *string* sem usar expressões regulares, use o pacote NuGet `InterpolatedParser`.
    *   Use `InterpolatedParser.Parse()` com uma *string* de entrada e uma *string* de modelo. Ao usar interpolação de *string* no modelo, você pode extrair variáveis da *string* de entrada. É "interpolação de *string* reversa".

*   **Dica 22: Alias para Qualquer Tipo (C# 12)**
    *   C# 12 introduz o recurso `alias any type`, que permite usar a diretiva `using` para dar um apelido a qualquer tipo no seu projeto.
    *   Isso resolve quatro problemas importantes:
        1.  **Simplifica nomes** para tipos longos ou complicados.
        2.  **Desambigua tipos** e resolve conflitos de nomes.
        3.  Permite definir **tipos *value tuple* compartilháveis** em um *assembly*.
        4.  Adiciona **clareza ao código** usando nomes mais descritivos.

*   **Dica 23: DateTimeOffset vs DateTime**
    *   Na maioria dos casos, use `DateTimeOffset` em vez de `DateTime`.
    *   `DateTime` representa apenas data e hora. A menos que seja `Kind.Utc`, pode se referir a qualquer fuso horário, o que é problemático para casos de uso de negócios.
    *   `DateTimeOffset` representa data e hora e também indica o quanto essa data e hora diferem do UTC. Assim, é sempre um momento exato no tempo. Isso evita ambiguidade e erros.

*   **Dica 24: Testes de Arquitetura (.NET Arc Test.Utilities)**
    *   Você pode restringir o uso de classes e impor políticas de arquitetura sem criar vários projetos.
    *   Use testes de arquitetura com o pacote NuGet `NetArchTest.Rules`.
    *   Use a interface `FluentBuilder` na classe `Types` para limitar o uso de classes entre *namespaces* ou definir quais tipos devem ser selados. Isso garante que *namespaces* de infraestrutura ou aplicação não usem classes incorretas.

*   **Dica 25: Alternativas ao Fluent Assertions (Licença Paga)**
    *   Se você usa *Fluent Assertions* e não quer pagar a taxa de licença, há opções:
        1.  **Trave na versão 7:** A mudança de licença se aplica apenas à versão 8 em diante.
        2.  **Awesome Assertions:** É um *fork* da versão gratuita de *Fluent Assertions* e um substituto direto.
        3.  **Shouldly:** Uma biblioteca similar ao *Fluent Assertions* que oferece funcionalidade semelhante com uma "redação" ligeiramente diferente.

*   **Dica 26: Exportando Json Schema no .NET 9**
    *   O .NET 9 introduziu um novo recurso para `System.Text.Json` chamado `JsonSchemaExporter`.
    *   Obtenha a classe `JsonSerializerOptions` e use o método `GetJsonSchemaNode()` para exportar o esquema JSON do seu objeto.
    *   Você pode usar opções personalizadas de `JsonSerializerOptions` e `JsonSchemaExporterOptions`. Esse é o recurso base da nova funcionalidade *OpenAPI* do .NET.

*   **Dica 27: Paralelismo Assíncrono (Parallel.ForEachAsync)**
    *   Para *loops* multi-threaded, desenvolvedores .NET costumavam usar os métodos `For` ou `ForEach` da classe `Parallel`. No entanto, usar `async` na *lambda* criava um método `async void`, o que é "um grande não".
    *   O .NET 6 adicionou os métodos `ForAsync` e `ForEachAsync` na classe `Parallel`. Eles se comportam como um `async Task` quando a palavra-chave `async` é usada, sendo uma ótima opção para paralelismo e concorrência.

*   **Dica 28: Retestando Testes Falhos (dotnet retest)**
    *   Para testes *flaky* (intermitentes), você pode usar a ferramenta CLI global `dotnet retest`.
    *   Instale-a globalmente e use `dotnet retest` para rodar seus testes. Ela tentará novamente automaticamente os testes *flaky* que falham algumas vezes, na esperança de que passem.
    *   **Nota importante:** A fonte enfatiza que você deve realmente consertar seus testes *flaky*.

*   **Dica 29: Params com Tipos Enumerable (C# 13)**
    *   A palavra-chave `params` permite passar qualquer quantidade de parâmetros para um método, mas era limitada a tipos *array* e tinha problemas de desempenho.
    *   No C# 13, `params` recebeu uma "atualização massiva". Agora você pode usá-lo com vários tipos `IEnumerable` (como `List`, `IEnumerable` e `Span`) e eles são significativamente mais performáticos e otimizados.

*   **Dica 30: O "Monitor" Maligno (Piada/Aviso)**
    *   **Esta é uma piada e não um conselho real:** Você pode criar um arquivo de classe com um nome aleatório e nomear a classe interna como `Monitor`, implementando métodos `Enter` e `Exit`.
    *   Se você atualizar o *namespace* dessa classe `Monitor` para `System.Threading`, o compilador usará sua classe em vez da classe `Monitor` nativa do .NET, fazendo com que o código entre nos seus próprios métodos `Enter` e `Exit` ao usar `lock`.
    *   **Aviso Legal:** A fonte deixa claro que isso é uma piada e não um conselho juridicamente vinculativo.

*   **Dica 31: Convenção de Underscore para Campos Privados**
    *   A convenção padrão para campos privados no .NET é iniciar com um *underscore* (`_`).
    *   O motivo é que o *underscore* define o escopo. Sem ele, em um método, seria difícil saber qual variável é um campo da classe global e qual é uma variável definida no método.
    *   Embora você possa usar `this.`, a recomendação é manter o *underscore*.

*   **Dica 32: Usando HttpClient Corretamente**
    *   **Não crie um novo `HttpClient` a cada requisição:** Isso pode causar exaustão de *sockets* (`socket exhaustion`).
    *   **Não use um `HttpClient` estático e o reutilize:** Isso resolve a exaustão de *sockets*, mas o DNS só é resolvido na instanciação. Se o DNS mudar, você "estará ferrado".
    *   **Solução:**
        1.  Use clientes de longa duração (`long-lived clients`) e defina o `PooledConnectionLifetime` para um valor adequado.
        2.  Ou, crie um `HttpClient` usando o `HttpClientFactory`. O *factory* descarta o cliente, mas reutiliza o `HttpMessageHandler`, resolvendo problemas de DNS e exaustão de *sockets*.

*   **Dica 33: Testes de Snapshot com Verify**
    *   *Snapshot testing* valida o resultado de uma ação de forma única, seja uma classe, JSON, texto, imagem ou UI.
    *   Usando a biblioteca `Verify`, você cria uma versão verificada do resultado. Cada vez que o teste é executado, a saída é comparada à versão original verificada.
    *   É uma maneira robusta de testar, perfeita para projetos sem testes de unidade, pois não é muito invasiva e é eficaz. Funciona com qualquer *framework* de teste e outras bibliotecas como *Entity Framework*.

*   **Dica 34: Chamando APIs com Refit**
    *   `Refit` permite que você crie interfaces com um método por requisição de API.
    *   Ao decorar o método com o atributo apropriado (`[Get]`, `[Post]`), `Refit` gera o resto do código para você.
    *   Use `RestService` ou `HttpClientFactory` para criar uma instância da interface.

*   **Dica 35: (Repetição da Dica 3) Travamento com SemaphoreSlim**
    *   Esta dica repete a Dica 3, fornecendo mais detalhes sobre a implementação do `SemaphoreSlim` para garantir que apenas uma *thread* acesse uma operação específica, especialmente útil com `async`/`await`. Recomenda-se adicionar um `timeout` no `WaitAsync` para evitar *deadlocks* acidentais.

*   **Dica 36: ULIDs (Sortable Unique Identifiers)**
    *   Além do `Guid v7` do .NET 9, soluções para IDs ordenáveis já existem há anos, como os ULIDs (`ULIDs` - *Universally Unique Lexicographically Sortable Identifier*).
    *   ULIDs são IDs gerados aleatoriamente, extremamente rápidos de criar e ordenáveis, úteis em sistemas distribuídos e computação em nuvem.
    *   Instale o pacote `Ulid` e use `Ulid.NewUlid()`. Eles podem ser convertidos de volta para `Guid`.

*   **Dica 37: Executando Operações Assíncronas em Paralelo (Task.WhenAll)**
    *   Se você tem múltiplas operações assíncronas que não dependem uma da outra, **não use `await` em série**. Isso lentifica o desempenho.
    *   Obtenha as `Tasks` desses métodos e **aguarde todas juntas usando `Task.WhenAll()`**. Isso permite que todas sejam executadas em paralelo, melhorando significativamente o desempenho.
    *   Você pode acessar a propriedade `Result` das `Tasks` diretamente após a conclusão de `WhenAll`, pois a tarefa já terá sido computada.

*   **Dica 38: Scoped Lifetimes Personalizados em DI**
    *   Além dos *lifetimes* `Transient`, `Scoped` e `Singleton`, você pode criar seus próprios *scopes* personalizados para casos de uso como processamento de mensagens.
    *   Injete `IServiceScopeFactory` e use o método `CreateScope()` para obter um `IServiceScope`. Isso permite acessar um `IServiceProvider` e obter um serviço dentro do seu *scope* personalizado.
    *   **Certifique-se de descartar (`Dispose`) o *scope*** quando o *lifetime* *scoped* não for mais necessário.

*   **Dica 39: (Repetição da Dica 12) Construtores Primários e Campos Readonly (Crítica Pessoal)**
    *   Esta dica reitera as desvantagens dos construtores primários: não permitem definir campos como `readonly`, e a sugestão de refatoramento dos IDEs pode mudar a semântica do código. Também menciona uma preferência pessoal por usar *underscore* para campos, o que conflita com a capitalização de parâmetros de construtor primário.

*   **Dica 40: Trabalhando com Unidades de Medida (UnitsNet)**
    *   Trabalhar com unidades de medida em C# pode ser confuso.
    *   A biblioteca `UnitsNet` (disponível via NuGet) adiciona muitas unidades e capacidades de conversão ao .NET (ex: metros para pés, RPM para torque).
    *   Ela oferece conversões explícitas e implícitas para as necessidades da sua aplicação.

*   **Dica 41: Validando o Container DI em Tempo de Compilação**
    *   Um dos maiores problemas com a Injeção de Dependência no .NET é que ela é em tempo de execução, então erros de configuração só aparecem quando a aplicação falha.
    *   Configure o container DI para "explodir cedo" (`blow up early`) em tempo de construção.
    *   Defina as configurações `ValidateScopes` e `ValidateOnBuild` do `ServiceProvider` como `true`. Qualquer configuração inimiga (`enemies configuration`) fará a construção falhar em vez de falhar em tempo de execução.

*   **Dica 42: Null Conditional Assignment (C# 14)**
    *   C# 14 está introduzindo o `null conditional assignment`.
    *   Atualmente, você pode usar o operador condicional nulo (`?.`) para acessar uma propriedade apenas se o operando não for nulo.
    *   No C# 14, você poderá ter uma verificação nula usando o operador condicional nulo e também atribuir um valor se o operando não for nulo.

*   **Dica 43: Inicializadores de Dicionário (Add vs Index Initializer)**
    *   C# 12 introduziu uma maneira mais limpa de inicializar dicionários vazios com `[]`.
    *   Existem duas maneiras semânticamente diferentes de configurar um dicionário:
        1.  **Sintaxe `Add` (C# 3):** Previne múltiplas instâncias da mesma chave e lança uma exceção se encontrada.
        2.  **Método de Inicialização por Índice (`Index Initializer`) (C# 6):** Permite chaves duplicadas e **silenciosamente sobrescreve** o valor da chave se definida múltiplas vezes.
    *   Esteja atento às diferenças e escolha sabiamente.

*   **Dica 44: Construtores Primários em Classes vs Records**
    *   **Records:** O construtor primário automaticamente cria propriedades públicas e imutáveis (`readonly`) usadas para igualdade baseada em valor.
    *   **Classes:** Os parâmetros do construtor primário estão no escopo da classe e podem ser usados em métodos ou inicializadores. Não há `this.name` ou propriedade pública `name` a menos que você declare uma. O compilador cria um campo secreto para armazenar o valor se ele for usado em um método.

*   **Dica 45: Ref Structs (Alto Desempenho e Segurança de Memória)**
    *   Um `ref struct` é um tipo especial de `struct` que deve permanecer na *stack* o tempo todo. Isso o torna "super rápido" e "super seguro" para cenários de alto desempenho. `Span<T>` é um exemplo de `ref struct`.
    *   **Restrições:** Não podem ser "boxados" (convertidos para `object`), usados em métodos `async` ou armazenados no *heap* (nem mesmo em campos de classes).
    *   Isso os torna perfeitos para processar grandes dados, parsing ou manipular memória com zero alocações.

*   **Dica 46: Palavra-chave 'in' (Passagem de Structs por Referência Readonly)**
    *   O uso mais comum da palavra-chave `in` é permitir passar `structs` por referência, mas tornando-os `readonly` dentro do método.
    *   Isso é para **desempenho**: copiar *structs* grandes pode ser caro. `in` evita isso passando uma referência, como `ref`, mas com uma rede de segurança (o método não pode modificá-lo).
    *   **Atenção:** Se sua *struct* não for marcada como `readonly`, C# pode criar cópias defensivas ocultas ao acessar seus membros, o que pode prejudicar o desempenho. Combine `in` com `readonly struct` ou `readonly members` para o desempenho real.

*   **Dica 47: Execução Adiante (Deferred Execution) do LINQ**
    *   Métodos LINQ como `Where`, `Select` e `Take` **não são executados imediatamente**. Eles apenas constroem uma consulta, como um "modelo".
    *   Isso significa que você pode continuar adicionando à sua consulta, até mesmo condicionalmente, depois de declarada.
    *   Quando você finalmente a executa (ex: com `ToList()`, `foreach`), C# a compila em uma única operação otimizada.
    *   **Cuidado:** Se seus dados mudarem antes da execução, isso aparecerá nos resultados. LINQ é "preguiçoso por design e poderoso quando você conhece as regras".

*   **Dica 48: Stackalloc (Alocação na Stack)**
    *   `stackalloc` permite alocar *arrays* na *stack* em vez do *heap*. Isso significa **sem coleta de lixo** e "desempenho incrivelmente rápido".
    *   A memória é liberada automaticamente quando o método termina, pois o *stack frame* é descartado.
    *   É "enorme" para desenvolvimento de jogos, *parsing* ou código de alto desempenho, pois evita o trabalho extra do coletor de lixo e pausas na aplicação.
    *   **Atenção:** O espaço da *stack* é limitado. Alocar demais pode causar um *stack overflow*.

*   **Dica 49: Tipos de Delegate Integrados (Func, Predicate, Action)**
    *   Delegates permitem armazenar e passar métodos como variáveis. Eles impulsionam eventos, *callbacks* e fluxo assíncrono.
    *   C# tem vários tipos de *delegate* integrados:
        1.  **`Func`:** Usado para métodos que retornam um valor (o último parâmetro de tipo é o tipo de retorno).
        2.  **`Predicate`:** Retorna especificamente um booleano. É basicamente um `Func<T, bool>` e é usado para lógica de filtragem. (Foi introduzido antes de `Func` e hoje você deve usar `Func<T, bool>`).
        3.  **`Action`:** Usado para métodos que retornam `void`.
    *   Você também pode encadear *delegates* juntos (multicast delegate) para invocar múltiplos métodos em sequência.

*   **Dica 50: Sobrescrita de Comportamento da Classe Base**
    *   C# oferece várias maneiras de o comportamento de uma classe base ser sobrescrito por uma classe derivada:
        1.  **Métodos `abstract`:** Devem ser sobrescritos. A classe base não tem lógica, forçando a definição em cada classe derivada.
        2.  **Métodos `virtual`:** Oferecem uma implementação padrão, mas permitem sobrescrição. Qualquer método que sobrescreve um `virtual` é ele próprio `virtual`, a menos que você o sele (`seal`).
        3.  **Métodos de interface:** São implicitamente `virtual`. No C# 8+, interfaces também podem ter implementações padrão que agem como métodos `virtual` quando chamados via interface.
        4.  **Ocultação de método (`method hiding`) usando `new`:** Não sobrescreve o método base, apenas o substitui na classe derivada quando acessado através do tipo derivado. Se você chamar através da referência da classe base, ainda obterá o método original.

*   **Dica 51: Reutilização de Arrays com ArrayPool.Shared**
    *   Para pular a coleta de lixo e reutilizar *arrays*, use `ArrayPool<T>.Shared`.
    *   Ele funciona como uma "lixeira de memória". Você aluga um *array* com `Shared.Rent()` quando precisa e o devolve com `Shared.Return()` quando termina.
    *   Isso evita a alocação de novos *arrays* a cada vez, perfeito para cenários de alto desempenho como *parsing* de JSON, E/S de arquivo ou manipulação de milhares de requisições por segundo.
    *   Sempre devolva o *array* depois de alugar. Se for armazenar dados sensíveis, passe `clearArray` como `true` ao retornar.

*   **Dica 52: Evitando Async Void (Armadilha)**
    *   `async void` é uma "armadilha" e deve ser evitado.
    *   Métodos `async void` não podem ser aguardados (`awaited`) corretamente como um `async Task`. Isso significa que não há tratamento de erros, controle de fluxo ou como saber quando a operação terminou. Uma exceção lançada dentro de um método `async void` fará a aplicação travar.
    *   **Único lugar onde `async void` é aceitável:** Manipuladores de eventos (`event handlers`). Manipuladores de eventos não retornam `Tasks`, então `async void` é a única opção, desde que você lide com as exceções internamente.

*   **Dica 53: Operador Null Forgiving (`!`)**
    *   O operador `null forgiving` (`!`) ("bang operator") informa ao compilador: "relaxe, eu sei que isso não será nulo em tempo de execução".
    *   Use-o quando você não sabe o valor da variável ou com a palavra-chave `default`.
    *   Com tipos de referência anuláveis ativados, C# exige prova de que uma propriedade não-anulável é inicializada. Se você sabe que será definida posteriormente pelo *framework*, construtor ou deserializador, use `!` para silenciar o aviso sem alterar o tipo.

*   **Dica 54: Usando a Palavra-chave `using` (e `await using`)**
    *   A palavra-chave `using` é um contrato que diz ao compilador: "quando eu terminar com isso, chame `Dispose()` para mim".
    *   `Dispose()` é chamado assim que um bloco `using` termina, mesmo que haja uma exceção.
    *   É perfeito para descartar arquivos, *streams*, conexões de banco de dados, *timers*, etc..
    *   Desde o C# 8, podemos ter `using` sem chaves (`{}`), onde o escopo é implícito.
    *   Também pode ser usado em um contexto assíncrono com `await using` ao trabalhar com `IAsyncDisposable`.
    *   O compilador traduz `using` para um bloco `try-finally`.

*   **Dica 55: Palavra-chave `with` (Clonagem Imutável)**
    *   A palavra-chave `with` permite clonar um objeto existente e alterar apenas o que você precisa, sem tocar no original.
    *   É perfeita para tipos imutáveis, onde você não quer que ninguém modifique um objeto após sua criação.
    *   `with` foi feito para tipos `record`. A partir do C# 10, também funciona para `structs`.
    *   Por baixo dos panos, C# gera automaticamente um método `Clone()` para `records`.
    *   **Cuidado:** Clonar uma classe realocará-a, pois é um tipo de referência.

*   **Dica 56: Membros de Extensão (Extension Members) - C# 14**
    *   No C# 14 (versão de *preview*), `extension methods` estão se tornando "obsoletos" (tipo).
    *   Agora você poderá adicionar métodos estáticos, métodos de instância, propriedades de instância e propriedades estáticas a qualquer tipo existente, **sem modificar o código-fonte original**, mesmo que você não seja o proprietário do objeto.
    *   Isso permite aprimorar qualquer tipo com nova funcionalidade, tornando o código mais expressivo e modular.
    *   Se você adotar esse recurso, migre seus `extension methods` existentes para ele.

*   **Dica 57: Expressões de Coleção (Collection Expressions) - C# 12**
    *   No C# 12, podemos usar expressões de coleção com colchetes (`[]`) para inicializar coleções.
    *   Isso simplifica a sintaxe, removendo a necessidade de `new List<T>()` ou chamadas `Add()`.
    *   Você também pode combinar coleções usando o operador *spread* (`..`).

*   **Dica 58: Suporte a Span para Params (C# 13)**
    *   A palavra-chave `params` permite um número variável de argumentos, mas geralmente significava uma alocação no *heap*.
    *   No C# 13, `params` ganhou suporte a `Span`. Como `Span` só pode ser alocado na *stack*, isso elimina a alocação no *heap*, tornando sua aplicação mais eficiente em termos de memória e mais rápida.

*   **Dica 59: Target-Typed New (C# 9)**
    *   C# 9 introduziu o `target-typed new`, permitindo que você omita o tipo ao criar objetos se o compilador já souber o tipo do lado esquerdo da atribuição.
    *   Isso funciona com construtores, coleções e até genéricos, resultando em um código mais limpo, especialmente em declarações complexas.

*   **Dica 60: Top-Level Statements (C# 9)**
    *   C# 9 removeu a necessidade do `public static void Main(string[] args)`.
    *   Você pode simplesmente digitar seu código diretamente no nível superior do arquivo `Program.cs`, e o compilador o envolverá em uma classe e um método por trás dos panos.
    *   Funciona até com `await`.
    *   **Atenção:** Você só pode ter um arquivo *top-level* por projeto; ele se torna seu ponto de entrada.

*   **Dica 61: Padrões `not`, `and`, `or` (Pattern Matching Aprimorado)**
    *   O *pattern matching* em C# ficou mais inteligente com o uso de `not`, `and` e `or`.
    *   Isso permite escrever código mais expressivo e declarativo, eliminando aninhamentos de `if` ou condições confusas.
    *   Use `not` para excluir padrões, `or` para combinar múltiplas opções e `and` para combinar condições em uma única linha. Funciona dentro de verificações e instruções `switch`.

*   **Dica 62: Usando `nameof` para Nomes de Símbolos**
    *   Evite usar *strings* codificadas (`hard-coded strings`) para representar nomes de código (ex: nomes de variáveis).
    *   Use `nameof()`. `nameof()` retorna o nome real do símbolo, então se você renomear o símbolo, seu código será atualizado automaticamente.
    *   É perfeito para *logging*, validação, exceções e atributos.

*   **Dica 63: Métodos Deconstruct em Qualquer Tipo**
    *   Você pode definir seu próprio método `Deconstruct()` em qualquer tipo em C#, não apenas em `records`.
    *   Adicione um método `Deconstruct` à sua classe e você pode desconstruir o objeto em variáveis separadas. Adicione sobrecargas para mais opções de desconstrução.

*   **Dica 64: Atributos em Expressões Lambda (C# 10)**
    *   Desde o C# 10, você pode adicionar atributos a expressões *lambda*.
    *   Isso é útil porque atributos podem adicionar metadados ou mudar o comportamento, mesmo em código *inline*.
    *   É especialmente útil para geradores de código-fonte, *middleware*, analisadores personalizados ou cenários de interoperabilidade.
    *   Por baixo dos panos, C# compila *lambdas* em métodos, e agora você pode adicionar atributos a esses métodos como se fossem regulares. Funciona com *lambdas* estáticas também.

*   **Dica 65: Pattern Matching com Ranges (Intervalos)**
    *   Além de verificar tipos com `is`, você pode verificar intervalos (ranges) com *pattern matching*.
    *   Ex: `if (x is > 0 and < 100)`. Use `>`, `<`, `>=`, `<=` e combine-os com `or` e `not`. Funciona com instruções `switch`.

*   **Dica 66: ArgumentNullException.ThrowIfNull (C# 10)**
    *   C# 10 introduziu `ArgumentNullException.ThrowIfNull()` para verificações de nulo mais limpas.
    *   Em vez de escrever verificações de nulo manuais e lançar `ArgumentNullException`, você pode usar `ArgumentNullException.ThrowIfNull(input)`.
    *   É uma linha, sem *boilerplate*, e o método nomeia automaticamente o argumento usando `nameof(input)`. Isso leva a construtores, serviços e *guards* de método mais claros.

*   **Dica 67: Construtores com Corpo de Expressão (Expression-Bodied Constructors)**
    *   Assim como métodos e propriedades, você pode ter construtores com corpo de expressão.
    *   É uma sintaxe de uma linha, sem chaves ou *boilerplate*, perfeita para tipos simples onde você está apenas atribuindo valores.
    *   A sintaxe pode ser "um pouco feia", então certifique-se de formatar seu código adequadamente para evitar declarações muito longas.

*   **Dica 68: Value Tuples vs Tuple (Class)**
    *   Quando você escreve uma *tuple* em C# (ex: `(int, string)`), por baixo dos panos, é uma `ValueTuple`.
    *   `ValueTuple` é uma `struct`, o que a torna leve e livre de alocações.
    *   Antes do C# 7, usávamos `Tuple<T1, T2>`, que vive no *heap* e não podia usar campos nomeados.
    *   Com `ValueTuple`, você pode escrever `(int id, string name)` e obter campos nomeados, *pattern matching* e melhor desempenho sem sintaxe extra ou classes.

*   **Dica 69: Palavras-chave Curiosas (Exemplo de Jogo de Palavras)**
    *   Esta dica é um jogo de palavras sobre algumas palavras-chave de C# (`in`, `out`, `short`, `try`, `catch`, `double`, `long`, `object`, `break`, `event`, `public`, `protected`), sem um conselho técnico direto.

*   **Dica 70: Passando `ref` para `in` (Legal, mas o Inverso Não)**
    *   É totalmente legal passar uma variável `ref` para um parâmetro `in`.
    *   `in` significa "somente leitura por referência" (`readonly by ref`). `ref` significa "variável por referência". O compilador aceita tratar uma referência modificável como uma referência somente leitura.
    *   **No entanto, o inverso não funciona:** Você não pode passar um parâmetro `in` para um método `ref`, porque `ref` significa que o método pode escrever nele.

*   **Dica 71: Comparando Tuplas em C#**
    *   Você pode comparar *tuples* em C# usando `==` ou `.Equals()`.
    *   C# compara *tuples* elemento por elemento, em ordem. Isso funciona porque `Tuple` implementa os operadores `Equals` e `NotEquals` e o método `Equals`.
    *   **Atenção:** Nomes não afetam a igualdade; apenas os valores importam.

*   **Dica 72: Atributos CallerMemberName, CallerFilePath, CallerLineNumber**
    *   Para que seus métodos saibam automaticamente quem os chamou sem usar reflexão (que é lenta), use os atributos `CallerMemberName`, `CallerFilePath` e `CallerLineNumber`.
    *   Aplique-os a um parâmetro de método (com um valor padrão, ex: `[CallerMemberName] string methodName = ""`).
    *   Sempre que você chamar o método, ele preencherá automaticamente o nome do método, o caminho do arquivo ou o número da linha em tempo de compilação.
    *   Perfeito para *logging*, notificações de mudança de propriedade e depuração.

*   **Dica 73: Atributos Semânticos (NotDisposed, DoesNotReturn, MaybeNull, NotNullWhenTrue)**
    *   C# possui atributos que fornecem ao compilador mais informações sobre seu código:
        1.  `NotNull`: Diz ao compilador que um parâmetro não será nulo após a execução do método.
        2.  `DoesNotReturn`: Marca métodos que nunca retornarão (ex: um método que sempre lança uma exceção). Ajuda o compilador a entender o fluxo sem mais avisos.
        3.  `MaybeNull`: Diz ao compilador que o método pode retornar nulo.
        4.  `NotNullWhenTrue`: Uma verificação de nulo inteligente ligada a valores de retorno.
    *   Esses atributos tornam sua intenção clara e ajudam o compilador a ser mais inteligente.

*   **Dica 74: O Uso de `var` (Readability vs Noise)**
    *   `var` pode esconder o tipo, tornando o código mais difícil de ler, especialmente com bibliotecas complexas ou desconhecidas.
    *   **Use `var` quando o tipo é óbvio** (ex: `var name = "Nick Chapsas";`).
    *   **Evite `var` quando ele esconde o significado** e prejudica a clareza.
    *   Não é sobre estilo ou opinião, mas sobre legibilidade e intenção.

*   **Dica 75: Evitando a Palavra-chave `dynamic`**
    *   Na maioria das vezes, você deve "fingir que a palavra-chave `dynamic` não existe".
    *   Ela oferece flexibilidade, mas a um custo: você perde verificação de tipo, desempenho e segurança de refatoração. Erros de digitação em propriedades só serão detectados em tempo de execução.
    *   `dynamic` também é mais lento, pois cada chamada envolve o DLR (`Dynamic Language Runtime`).
    *   **Quando `dynamic` é aceitável:** Interoperabilidade COM, trabalhar com JSON ou `ExpandoObject`, código *ad hoc* ou comunicação com um motor de *scripting* (Python, JavaScript) – "código de cola".
    *   Em sua lógica de aplicação principal, use tipagem forte, interfaces, *records* e classes.

*   **Dica 76: Exceções para Casos Excepcionais, Não Lógica Normal**
    *   Usar exceções para lógica de negócios normal é um "mau design".
    *   Exceções são caras (criam *stack traces*, afetam o GC e lentificam) e escondem sua intenção.
    *   Use exceções apenas para erros e casos **excepcionais**, não para condições esperadas (ex: verificar se um usuário existe).

*   **Dica 77: Testes Unitários Assíncronos Devem Retornar `Task`**
    *   **Nunca use `async void` em testes unitários**. Seus *frameworks* de teste (xUnit, NUnit, MSTest) não podem rastrear métodos `async void`.
    *   `async void` não retorna uma `Task`, então o *test runner* pode executá-lo e prosseguir antes que o código interno termine, ou pior, pode perder completamente uma exceção lançada.
    *   Para corrigir, **sempre retorne uma `Task`**. Todos os principais *frameworks* de teste funcionam assim. `async void` é apenas para *event handlers*.

*   **Dica 78: Evitando `region` (Estrutura de Código)**
    *   Usar `#region` para organizar o código é como "dobrar a roupa e enfiá-la debaixo da cama". Pode parecer limpo, mas ainda é uma bagunça.
    *   A solução real é **quebrar o arquivo ou organizá-lo melhor** com métodos. Divida classes grandes em menores, extraia a lógica em componentes focados.
    *   Se você precisa de `region` para gerenciar seu código, sua classe provavelmente tem muitas responsabilidades.
    *   `region` é um "curativo", não uma estrutura. Além disso, só funciona em IDEs; em revisões de código no GitHub, é inútil.

*   **Dica 79: Usando o Atributo `Obsolete` Corretamente**
    *   Adicionar o atributo `[Obsolete]` parece responsável, mas se você não planeja remover o método, está apenas gerando "ruído de aviso".
    *   Equipes de desenvolvimento podem ignorar avisos, e o código permanece em produção por anos, tornando o aviso sem sentido.
    *   **Use `[Obsolete]` apenas quando você está realmente deprecando, com remoção agendada e um substituto claro e testado**. Caso contrário, use um comentário.

*   **Dica 80: Rodando um Arquivo C# Direto do Console**
    *   Você pode rodar um arquivo C# diretamente do console sem criar um projeto completo.
    *   Basta usar `dotnet run app.cs`.
    *   Você pode adicionar configurações no topo do arquivo (ex: `// <enable/> // <sdk>Microsoft.NET.Sdk</sdk> // <package>...</package>`).
    *   É possível usar sintaxe *shebang* para torná-lo executável (`#!/usr/bin/env dotnet-script`) e conceder permissões.

*   **Dica 81: Global Usings (C# 10)**
    *   Desde o C# 10, você pode mover suas declarações `using` repetitivas para um único arquivo `GlobalUsings.cs`.
    *   Agora, cada arquivo em seu projeto terá essas declarações `using` automaticamente.
    *   Funciona tanto para *namespaces* quanto para importações estáticas.

*   **Dica 82: `nameof` Não é Reflexão (É Mais Rápido)**
    *   `nameof()` **não é reflexão**. Ele retorna o nome de uma *string* em tempo de compilação.
    *   Isso significa **zero custo em tempo de execução**, sem alocações e sem penalidade de desempenho.
    *   Compare isso com `GetType().Name` ou APIs de reflexão, que são executadas em tempo de execução e têm custo de desempenho significativo, podendo também quebrar alguns cenários AOT (*ahead-of-time*). Use `nameof` com segurança.

*   **Dica 83: InternalsVisibleTo (Testando Membros Internos)**
    *   Se você tem classes ou métodos internos que deseja testar, **não precisa torná-los públicos**.
    *   Use o atributo `InternalsVisibleTo`. Aplique-o no `AssemblyInfo.cs` ou diretamente como um atributo de *assembly*.
    *   Isso permite que seu projeto de teste acesse membros internos. **Não confunda com membros privados**, que são diferentes e devem ser testados através de seus membros públicos.

*   **Dica 84: Filtros `when` em Blocos `catch`**
    *   Você pode limpar o tratamento de exceções usando filtros `when` nos blocos `catch`.
    *   Ex: `catch (HttpException ex) when (ex.StatusCode == 404)`. Isso torna o código muito mais claro do que usar instruções `if` dentro da cláusula `catch`.

*   **Dica 85: Restrição de Tipo Genérico `notnull`**
    *   Além de `where T : class` ou `where T : struct`, você pode usar `where T : notnull` em genéricos.
    *   `notnull` significa que o tipo não pode ser nulo. Funciona com tipos de referência não anuláveis e tipos de valor não anuláveis (ex: `string`, `int`, `DateTime`).
    *   Não funciona com `nullable string`, `nullable int`, etc..
    *   Isso permite escrever APIs seguras e livres de nulos sem forçar uma divisão entre `struct` ou `class`. Ótimo para chaves de dicionário, IDs e objetos de valor.

*   **Dica 86: Escopo de Valores com `scoped` (C# 12)**
    *   No C# 12, a palavra-chave `scoped` se tornou parte da linguagem.
    *   `scoped` significa que um valor **não deve sobreviver ao chamador**. Isso impede alocação no *heap*, captura acidental ou escape.
    *   É perfeito para trabalhar com tipos somente de *stack* como `Span<T>` ou `ref struct`.
    *   Protege contra a escrita de código que compila, mas travaria em tempo de execução.
    *   Funciona não apenas para parâmetros, mas também com *ref locals*. O compilador impõe a segurança do escopo.

*   **Dica 87: Destrutores (`~`) e `IDisposable`**
    *   Destrutores (sintaxe `~ClassName()`) em C# são chamados de *finalizers*. Eles são executados quando o coletor de lixo decide que seu objeto não é mais necessário.
    *   **Problemas:** Você não controla quando ele é executado, não é executado no desligamento da aplicação e roda em uma *thread* em segundo plano sem garantias. Além disso, adicionar um *finalizer* torna o GC mais lento.
    *   **Quando usar:** Quase nunca. **Prefira `IDisposable` para descarte simples**. Use destrutores apenas para liberar e gerenciar recursos **no caso de `Dispose()` nunca ter sido chamado**.

*   **Dica 88: Property Patterns (Padrões de Propriedade)**
    *   Em vez de verificações de nulo aninhadas e instruções `if`, use *property patterns*.
    *   Isso permite corresponder profundamente dentro de um objeto, diretamente na condição.
    *   Você pode corresponder valores ou combinar padrões para condições complexas.

*   **Dica 89: List Patterns (C# 11)**
    *   C# 11 introduziu *list patterns*, que permitem corresponder *arrays* e *lists* pela "forma" (`shape`).
    *   Ex: `` verifica se a lista tem exatamente três elementos com esses valores.
    *   Você pode corresponder partes e ignorar o resto com `..` (um "curinga" para qualquer número de elementos).
    *   Pode capturar o último elemento (`[.., last]`) ou uma combinação.
    *   É um pouco limitado em funcionalidade, mas útil.

*   **Dica 90: Sealed Override (Selando Sobrescrições)**
    *   Além de `override`, existe `sealed override`.
    *   Isso significa que uma sobrescrição **não pode ser sobrescrita novamente**.
    *   Se uma classe base define um método `virtual` e você o sobrescreve na subclasse, você pode "travá-lo" com `sealed` sem selar a classe inteira.
    *   Isso previne comportamento inesperado em futuras classes derivadas e é especialmente útil ao escrever *frameworks* ou bibliotecas.

*   **Dica 91: Métodos `TryParse` (Controle de Fluxo Limpo)**
    *   Para analisar um número ou qualquer coisa que possa falhar, use métodos `TryParse` (ex: `int.TryParse`).
    *   Métodos `TryParse` **não lançam exceções**. Eles fornecem um fluxo de controle limpo, sem surpresas.
    *   Adicione o sufixo `Try` ao nome do seu próprio método, retorne um booleano e use um parâmetro `out`. Isso resulta em lógica rápida e previsível, sem `try-catch` ou *stack traces*.

*   **Dica 92: Experimentando Recursos C# Futuros (`LangVersion Preview`)**
    *   Você pode experimentar os recursos mais recentes do C# antes que sejam oficialmente lançados usando `LangVersion Preview` no seu arquivo `.csproj`.
    *   Isso permite escrever código C# usando os recursos da próxima versão (ex: *extension members* e *discriminated unions*).
    *   É excelente para testar recursos e fornecer feedback, mas tenha em mente que eles podem mudar antes do lançamento oficial.

*   **Dica 93: Índices a Partir do Fim (`^` operator)**
    *   Para obter o último item de um *array* de forma performática, você não precisa mais de `length - 1`.
    *   Use a sintaxe `^1`, que significa "o item um a partir do final". `^2` é o segundo ao último, e assim por diante.
    *   É chamado de "índice a partir do fim" (`index from end`) e é "super limpo", embora um pouco difícil de ler inicialmente. Funciona com *arrays*, `Spans` e *strings*.

*   **Dica 94: Literais de Array Vazios Otimizados (C# 12)**
    *   Você pode criar um *array* vazio simplesmente escrevendo `[]` (colchetes vazios).
    *   No C# 12, este literal de *array* vazio é totalmente otimizado sob o capô. Ele reutiliza a mesma instância estática, assim como `Array.Empty<T>()`, oferecendo o mesmo desempenho, mas sendo mais limpo e legível.

*   **Dica 95: await foreach (Iteração Assíncrona)**
    *   Para iterar dados assíncronos (como APIs paginadas ou *streams* de arquivos), você não precisa de um *loop* cheio de `await`.
    *   Use `await foreach`. Por exemplo, um método que retorna um `IAsyncEnumerable<T>` é um *stream* que você pode consumir à medida que os itens chegam, sem *buffering*, bloqueio ou "magia".
    *   É perfeito para grandes conjuntos de dados, filas de mensagens ou *pipelines* reativos, e também lida com cancelamento.

*   **Dica 96: Palavra-chave `checked` (Detecção de Overflow Matemático)**
    *   Operações matemáticas em C# (ex: `int.MaxValue + 1`) **não lançam um erro por padrão**; elas simplesmente "enrolam" para um número negativo silenciosamente.
    *   Você pode detectar isso com a palavra-chave `checked`.
    *   `checked` força o tempo de execução a validar sua matemática, e se ocorrer um *overflow*, ele lança uma exceção.
    *   Você pode envolver blocos inteiros de código com `checked`.
    *   Use-o para cálculos financeiros, simulações de física ou qualquer lugar onde erros matemáticos sejam críticos.

*   **Dica 97: Igualdade de Objetos Anônimos (`Equals()` Sobrescrito)**
    *   Dois objetos anônimos com os mesmos valores são considerados iguais se você usar `Equals()`, mas não `==` (que verifica a igualdade de referência).
    *   Isso ocorre porque tipos anônimos em C# automaticamente **sobrescrevem os métodos `Equals()` e `GetHashCode()`** com base nos valores das propriedades.
    *   É perfeito para estruturas de dados temporárias, como agrupamento em LINQ ou comparações, sem escrever *boilerplate*.

*   **Dica 98: Task.Yield() (Yielding Control)**
    *   `Task.Yield()` não espera; ele apenas diz ao tempo de execução para "pausar aqui e retomar mais tarde".
    *   Isso significa **devolver o controle ao chamador**, evitar bloqueios e permitir que a UI ou o *event loop* "respirem".
    *   É perfeito quando você quer devolver o controle à *thread* da UI, evitar que um método de longa duração congele a aplicação, ou quebrar trabalho síncrono em partes assíncronas.
    *   **Não atrasa** como `Task.Delay`; não há *timers* ou "sono", apenas uma troca de contexto.

*   **Dica 99: Inlining de Métodos (Method Inlining)**
    *   Métodos pequenos são frequentemente "inlineados" pelo JIT (*Just-In-Time* compiler). Isso significa que a chamada ao método é substituída pelo corpo do método, eliminando a *call stack* e a sobrecarga.
    *   Às vezes, o JIT não fará o *inlining* a menos que você o "empurre".
    *   Use o atributo `[MethodImpl(MethodImplOptions.AggressiveInlining)]` para dizer ao compilador para *inlinear* o método "se for possível".
    *   Ótimo para bibliotecas micro-otimizadas, *loops* com cálculos pesados ou "caminhos quentes" (*hot paths*).
    *   **Não use em todo lugar:** *Inlining* de métodos grandes pode prejudicar o desempenho e o tamanho do código.

*   **Dica 100: Expressões Regulares Compiladas em Tempo de Compilação (.NET 7+)**
    *   Expressões regulares escritas como *strings* (`new Regex("pattern")`) são compiladas em tempo de execução e consomem memória.
    *   A partir do .NET 7, você pode gerar *regexes* em tempo de compilação.
    *   Use o atributo `[GeneratedRegex("pattern")]` em um método parcial que retorna `Regex`.
    *   Isso resulta em *regexes* normais, sem alocações, sem atraso do JIT. A *regex* é "incorporada" ao seu *assembly*, tornando-a rápida, reutilizável e totalmente estática.