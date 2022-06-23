# AlgebraicEffects for C#

## これは何？

Algebraic Effects という考え方があると知ったので理解を深めるために実際に実装を考えてみました。

本来は言語レベルで新しい構文として実装されるべき物らしいので、実装としてはかなり無理矢理になります。

## 何ができるの？

例外処理のように呼び出しコンテキストを遡り、振る舞いを解決します。
ようは、以下のように振る舞いの一部を関数（またはクラス）の外部で解決させます。

```csharp
int SomeGetterFunction()
{
    // ... 処理 ... //
    if (isSucceeded)
    {
        // .. 成功時の処理 .. //    
        return succesCode;
    }
    else // .. 失敗時の処理 .. //
    {
        return new IntPerform();   
    }
}

void Main()
{
    using (new EffectHandler()
               .Handle((IntPerform _) => 100))
    {
        int ret = SomeGetterFunction();    
        // 失敗時は 100 が取得される
    }
}
```

## 基本的な使い方

### Perform

振る舞いを外部で解決したい場合に、`Exception`のように`new`します。

生成された`Perform`クラスのインスタンスは戻り値の型に代入される事で、振る舞いの解決を行います。

```csharp

// 生成しただけでは解決されない
new IntPerform();

// 戻り型に代入が発生した時に解決される
int ret = new IntPerform();

// Perform の型で受け取っただけでも解決はされない
var perform = new IntPerform();
// 型変換時に実行される
int ret2 = perform;

```
### EffectHandler

振る舞いを解決する側は、`EffectHandler`を`using`宣言と共に利用します。

`EffectHandler`の`Handle`メソッドによって受け取りたい`Perform`型と対応する戻り値を返すハンドラーメソッドを登録します。

```csharp
using (new EffectHandler()
           .Handle((IntPerform _) => 100)) // 解決
{
    // IntPerform が発生しうる処理
    int ret = DoSomething();
}
```

複数の`Perform`型に対応するメソッドを登録する事も可能です。

その場合は型がマッチするハンドラーが呼び出されます。

```csharp
using (new EffectHandler()
           .Handle((IntPerform _) => 100) // 解決
           .Handle((StrPerform _) => "文字列"))
{
    // IntPerform が発生しうる処理
    // 呼び出されるのは 100 を返すハンドラー
    int ret = DoSomething();
}
```

`using`宣言は入れ子に宣言する事が可能です。

その場合は最も最後に登録されたハンドラーが呼び出されます。

```csharp
using (new EffectHandler()
           .Handle((IntPerform _) => 100))
{
    using (new EffectHandler()
               .Handle((IntPerform _) => 50)) // 解決
    {
        // IntPerform が発生しうる処理
        // 呼び出されるのは 50 を返すハンドラー
        int ret = DoSomething();
    }    
}
```

直近の`using`宣言が対応する`Perform`型のハンドラーを定義していない場合は、登録順序を遡って解決をします。

```csharp
using (new EffectHandler()
           .Handle((IntPerform _) => 100)) // 解決
{
    using (new EffectHandler()
               .Handle((StrPerform _) => "文字列"))
    {
        // IntPerform が発生しうる処理
        // 呼び出されるのは 100 を返すハンドラー
        int ret = DoSomething();
    }    
}
```

## Perform with Message

`Perform`型のコンストラクタは単一の`object`型の引数を受け取ります。

この値は、ハンドラー側で`Message`プロパティとして受け取る事ができます。

```csharp
int SomeGetterFunction()
{
    // ... 処理 ... //
    if (isSucceeded) // .. 成功時の処理 .. //    
    {
        return new IntPerform(true);   
    }
    else // .. 失敗時の処理 .. //
    {
        return new IntPerform(false);   
    }
}

void Main()
{
    using (new EffectHandler()
               .Handle((IntPerform p) => 
               {
                   if (p.Message is bool flag && flag)
                   {
                       return 0;                    
                   }
                   return 1;                   
               }))
    {
        int ret = SomeGetterFunction();    
        // 失敗時は 100 が取得される
    }
}
```

## Async Handler (Experimental)

`EffectHandler`の`HandleAsync`メソッドを利用する事で非同期メソッドの登録も可能です。

`Perform`型のインスタンスを`ValueTask`型に変換した際、非同期メソッドのハンドラーが呼ばれます。

```csharp

async ValueTask<string> DoSomethingAsync()
{
    // ... 処理 ... //
    return new StrPerform();
}

await using (new EffectHandler()
                 .HandleAsync(async (StrPerform _) =>
                 {
                     await Task.Delay(10);
                     return "非同期文字列";
                 }))
{
    await Task.Delay(10);
    var ret = await DoSomethingAsync();
    // "非同期文字列"を取得
}
```

`Perform`型のインスタンスを`ValueTask`型に変換したものの、非同期ハンドラーが登録されていない場合は、同期のハンドラーが呼び出されます。

```csharp

async ValueTask<string> DoSomethingAsync()
{
    // ... 処理 ... //
    return new StrPerform();
}

await using (new EffectHandler()
                 .Handle((StrPerform _) => "同期文字列"))
{
    await Task.Delay(10);
    var ret = await DoSomethingAsync();
    // "同期文字列"を取得
}
```