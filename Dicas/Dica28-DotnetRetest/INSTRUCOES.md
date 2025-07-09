# Como Demonstrar a Dica 28: dotnet retest

## 🚀 Configuração Inicial

### 1. Instalar dotnet-retest
```bash
dotnet tool install -g dotnet-retest
```

### 2. Verificar instalação
```bash
dotnet-retest --version
```

## 📝 Comandos de Demonstração

### Executar Testes Estáveis (sempre passam)
```bash
dotnet test --filter "Category=Stable"
```

### Executar Testes Flaky (podem falhar)
```bash
dotnet test --filter "Category=Flaky"
```

### Usar dotnet retest para Testes Flaky
```bash
# Retry até 3 vezes
dotnet retest --retry-count 3 --filter "Category=Flaky"

# Com output verbose
dotnet retest --retry-count 5 --verbose --filter "Category=Flaky"

# Com delay entre tentativas
dotnet retest --retry-count 3 --delay 1000 --filter "Category=Flaky"
```

### Executar Todos os Testes com Retest
```bash
dotnet retest --retry-count 5 --verbose
```

## 🧪 O que Você Verá

### Testes Estáveis
- ✅ `ProcessData_FixedTest_ShouldPass` - sempre passa
- ✅ `NetworkCall_WithTimeout_ShouldHandleFailures` - trata falhas corretamente
- ✅ `IsolatedState_StableTest_ShouldPass` - sem estado compartilhado

### Testes Flaky
- ❌ `ProcessData_FlakyTest_MayFail` - race condition
- ❌ `FlakyOperation_MayFail` - falha ~30% das vezes
- ❌ `NetworkCall_FlakyTest_MayFail` - falha de rede simulada
- ❌ `SharedState_FlakyTest_MayFail` - estado compartilhado

### Com dotnet retest
- 🔄 Testes flaky são reexecutados automaticamente
- 📊 Relatório mostra quantas tentativas foram necessárias
- ⚠️ Alguns ainda podem falhar após várias tentativas

## 💡 Lições Aprendidas

1. **dotnet retest é uma ferramenta de diagnóstico**, não uma solução
2. **Sempre corrija a causa raiz** dos testes flaky
3. **Use timeouts apropriados** em operações assíncronas
4. **Evite estado compartilhado** entre testes
5. **Aguarde operações assíncronas** corretamente com await

## 🎯 Script Automático

Execute o script de demonstração:
```bash
./demo.sh
```

Este script executará todos os cenários automaticamente e mostrará os resultados.
