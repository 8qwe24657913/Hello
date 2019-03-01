# Hello.2019CsDotNet

C# 课程作业

### 前后对比

修改前：

![](.\picture\screencut-hello.PNG)

修改后：

![](.\picture\solved.png)

### 对各种实现方式的比较：

为了恢复原本的颜色，最简单的方法是调用 `Console.ResetColor()` 方法，但这样会带来的问题是：之前设置过的颜色会在重置时丢失，于是无法分别重置前景色与背景色，也无法嵌套使用：

```C#
// 方法一: ResetColor
Console.BackgroundColor = ConsoleColor.Green;
Console.WriteLine("Hello World!");

Console.ForegroundColor = ConsoleColor.Yellow;
Console.Write("Hello ");
Console.ResetColor(); // 绿色背景也被一起重置了，无法单独重置

// 输出红色的 "World" 字样
Console.ForegroundColor = ConsoleColor.Red;
Console.Write("World");
Console.ResetColor(); // 前景色变为默认而非之前的黄色，无法嵌套使用

Console.WriteLine("!");
```

为此，我们可以提前保存原来的颜色，并手动设置回去：

```C#
// 方法二: 保存上一次的结果，嵌套层级太多的话也可以用栈
var bgColor = Console.BackgroundColor;
Console.BackgroundColor = ConsoleColor.Green;
Console.WriteLine("Hello World!");
Console.BackgroundColor = bgColor; // 背景色改为原来的颜色，不修改前景色，且可以嵌套使用
```

看上去效果很不错，但也会有两个问题：遗忘和异常

```C#
var bgColor = Console.BackgroundColor;
Console.BackgroundColor = ConsoleColor.Green;
Console.WriteLine("a bunch of lines");
// a bunch of lines...
DoSomethingDangerous(); // 如果抛出异常则不会继续执行，也就不会恢复颜色了
// a bunch of lines...
Console.WriteLine("a bunch of lines");
Console.BackgroundColor = bgColor; // 程序员可能由于间隔太远而忘记写这一句，而且编译时不会报错
```

于是我们可以考虑利用 `using` 语句来解决这个问题（直接用 `try-finally` 也一样，但 `using` 方便封装，详见 `ChangeColor.cs`）：

```C#
// 方法三: using statement
using (new ChangeColor.Back(ConsoleColor.Green)) {
    Console.WriteLine("green background");
    using (new ChangeColor.Fore(ConsoleColor.Yellow)) {
        Console.WriteLine("green background & yellow text");
    }
    Console.WriteLine("green background");
}
Console.WriteLine("initial style");
```



### 延伸讨论：跨语言的"清理"方法比较

"清理"方法是一种比较常见的方法，多见于做一件有副作用的工作后需要消除副作用时，例如资源分配后的释放

#### C

完全手动清理，`goto` 的正确使用姿势

```C
void fn() {
    struct Resource *res = getRes();
    // a bunch of lines...
    int errcode = doSth();
    if (errcode != SUCCESS) goto cleaning;
    // a bunch of lines...
    cleaning:
    free(res); // do cleaning here
}
```

#### C++

可以利用 `RAII` 引入的析构函数（如果仅是简单的资源释放，可以直接使用 `unique_ptr` 等智能指针，原理相同）

```c++
class Guard {
    const function<void()> &guardFn;
    Guard(const function<void()> &guardFn):guardFn(guardFn) {}
    ~Guard() {
        guardFn();
    }
}
void fn() {
    auto guardFn = [&]() {
        // do cleaning here
    };
    Guard guard(guardFn); // 函数结束执行析构函数时调用 guardFn
    // a bunch of lines...
}
```

#### Java

`try-with-resources` 语句，需要实现 `AutoCloseable` 接口，`catch` 块和 `finally` 块均可选

```Java
public static class Resource implements AutoCloseable {
    @Override
    public void close() throws Exception {
        // do cleaning here
    }
}
class Program {
    void fn() {
        try (var res = new Resource()) {
            // use res here
        } catch (SomeException e) {
            // (optional) error handling here, after close()
        } finally {
            // (optional) other works here, after close()
        }
    }
}
```

#### Python

`with` 语句，鸭子类型无需实现接口，比 Java 多了一个准备的魔术方法，除清理工作外，将错误也交由 `__exit__` 魔术方法处理

```python
class Resource():
    def __enter__(self):
        # do prepareing here
        return self
    def __exit__(self, exc_type, exc_value, exc_traceback):
        # do cleaning here

def fn():
    with Resource() as res:
        # do sth with res

```

#### C#

`using` 语句，需实现 `IDisposable` 接口

```c#
public class Resource : IDisposable {
    public void Dispose() {
        // do cleaning here
    }
}

class Program {
    void Fn() {
        using (var res = new Resource()) {
            // use res here
        }
    }
}
```

#### Rust

（作者并没有学过 Rust，如有错误请指出）Rust 使用 `drop trait` 来完成类似于 C++ 析构函数的功能（代码参考自 [Rust 官网文档](https://doc.rust-lang.org/book/ch15-03-drop.html)）

```Rust
struct CustomSmartPointer {
    data: String,
}

impl Drop for CustomSmartPointer {
    fn drop(&mut self) {
        println!("Dropping CustomSmartPointer with data `{}`!", self.data);
    }
}

fn main() {
    let c = CustomSmartPointer { data: String::from("my stuff") };
    let d = CustomSmartPointer { data: String::from("other stuff") };
    println!("CustomSmartPointers created.");
}
```

#### Go

（作者并没有学过 Go，如有错误请指出） Go 提供 `defer` 关键字，不管程序是否出现异常，均在函数退出时执行代码

```go
func fn(){
    file, err := os.Open("myfile.txt")
    if err != nil {
        fmt.Println("open file failed:", err)
        return
    }
    defer file.Close()
     // a bunch of lines...
}
```

