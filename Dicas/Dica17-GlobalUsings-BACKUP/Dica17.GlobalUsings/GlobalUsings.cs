// =================== GLOBAL USINGS ===================
// Este arquivo define usings que serão aplicados automaticamente 
// a todos os arquivos .cs do projeto

// Usings do sistema básico
global using System;
global using System.Collections.Generic;
global using System.Linq;
global using System.Threading.Tasks;
global using static System.Console;

// Usings para I/O e serialização
global using System.IO;
global using System.Text.Json;
global using System.Text.Json.Serialization;

// Usings para collections avançadas
global using System.Collections.Concurrent;
global using System.Collections.Immutable;

// Usings para diagnósticos e performance
global using System.Diagnostics;
global using System.Runtime.CompilerServices;

// Aliases globais para tipos comumente usados
global using JsonOptions = System.Text.Json.JsonSerializerOptions;
global using StringDict = System.Collections.Generic.Dictionary<string, string>;
global using IntList = System.Collections.Generic.List<int>;

// Aliases para delegates comuns
global using StringAction = System.Action<string>;
global using StringFunc = System.Func<string, string>;
global using AsyncStringFunc = System.Func<string, System.Threading.Tasks.Task<string>>;
