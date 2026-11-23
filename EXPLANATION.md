# 📘 شرح مشروع Linear Algebra Solver — من الصفر للاحتراف

> مشروع C# / .NET 10 / WPF — Layered Architecture — OOP — Design Patterns

---

## 📌 الفكرة العامة

البرنامج بيستقبل من المستخدم مصفوفات أو أنظمة معادلات، وبيحلها بشكل دقيق **بدون أي خطأ عشري (floating-point)**، وبيعرض الحل خطوة بخطوة، وبيحتفظ بـ History للعمليات السابقة.

```
المستخدم يكتب المصفوفة
        ↓
  WPF UI (MainWindow)
        ↓
   SolverService         ← طبقة Application
   /           \
ISolver    IHistoryRepo  ← Domain Interfaces
   |               |
GaussJordan   JsonHistory  ← implementations
```

---

## 🏗️ المعمارية: Layered Architecture

المشروع مقسّم لـ **4 طبقات** وكل طبقة لا تعرف عن اللي فوقها:

```
┌─────────────────────────────────────────────┐
│          UI Layer  (WPF)                    │
│  MainWindow.xaml + MainWindow.xaml.cs       │
└──────────────────┬──────────────────────────┘
                   │ تستدعي
                   ▼
┌─────────────────────────────────────────────┐
│          Application Layer                  │
│   SolverService + HistoryService            │
│   EquationParser                            │
└────────┬───────────────────────┬────────────┘
         │                       │
         ▼                       ▼
┌──────────────────┐   ┌──────────────────────┐
│   Domain Layer   │   │  Infrastructure      │
│  Fraction        │   │  JsonHistoryRepo     │
│  Matrix          │   │  TextHistoryRepo     │
│  Solvers         │   └──────────────────────┘
│  Interfaces      │
└──────────────────┘
```

> [!NOTE]
> **القاعدة الذهبية:** Domain لا يعرف عن Infrastructure أو UI أبداً.
> UI لا تتكلم مع Solvers مباشرة — دايماً عن طريق SolverService.

---

## 📂 شرح كل ملف بالتفصيل

---

### 🔷 1. `Fraction.cs` — الكسور الدقيقة

**الهدف:** تخزين الأرقام كـ كسور (بسط / مقام) بدل الأرقام العشرية.

**ليه؟** لأن `0.1 + 0.2 = 0.30000000004` في الحاسوب. أما `1/10 + 2/10 = 3/10` دقيق 100%.

```csharp
// بدل: double x = 1.0 / 3.0;  // = 0.3333333...
// نستخدم:
Fraction x = new Fraction(1, 3);  // = 1/3 بالضبط
```

**أهم المفاهيم فيه:**

```csharp
public readonly struct Fraction   // struct مش class (value type — أسرع في الذاكرة)
{
    public long Numerator   { get; }     // البسط
    public long Denominator { get; }     // المقام
```

**التبسيط التلقائي (GCD):**
```csharp
// لما تكتب new Fraction(6, 4) → يتحول لـ 3/2 تلقائياً
long gcd  = Gcd(Math.Abs(numerator), Math.Abs(denominator));
Numerator   = sign * numerator   / gcd;
_denominator = sign * denominator / gcd;
```

**الحيلة الذكية — الـ default struct:**
```csharp
// C# بيخلي default(Fraction) = (0, 0) وده مشكلة (قسمة على صفر)
// الحل:
public long Denominator => _denominator == 0 ? 1 : _denominator;
// لو المقام صفر (من default) → يرجع 1 تلقائياً
```

**العمليات:**
```csharp
// كل + - * / معرّفة بـ operator overloading
public static Fraction operator +(Fraction a, Fraction b) =>
    new(a.Numerator * b.Denominator + b.Numerator * a.Denominator,
        a.Denominator * b.Denominator);
// 1/2 + 1/3 = (1*3 + 1*2) / (2*3) = 5/6
```

**Parser — يحوّل النص لكسر:**
```csharp
Fraction.Parse("3")    // → 3/1
Fraction.Parse("-2/5") // → -2/5
Fraction.Parse("0.75") // → 3/4
```

---

### 🔷 2. `Matrix.cs` — المصفوفة

**الهدف:** تخزين مصفوفة من `Fraction` وتوفير عمليات الصفوف اللي بيحتاجها Gauss-Jordan.

```csharp
public class Matrix
{
    private readonly Fraction[,] _data;  // المصفوفة الداخلية — private
    public int Rows { get; }             // عدد الصفوف
    public int Cols { get; }             // عدد الأعمدة
```

**Encapsulation:** `_data` مخفية، المستخدم يوصلها بس عن طريق الـ indexer:
```csharp
public Fraction this[int row, int col]
{
    get => _data[row, col];
    set => _data[row, col] = value;
}
```

**عمليات الصفوف (Row Operations):**
```csharp
SwapRows(r1, r2)              // تبديل صفين: R1 ↔ R2
ScaleRow(r, scalar)           // ضرب صف في عدد: R1 = k × R1
AddScaledRow(dest, src, k)    // إضافة مضاعف صف لآخر: Rd = Rd + k × Rs
```

**ضرب المصفوفات:**
```csharp
// C[i][j] = مجموع A[i][k] × B[k][j]
for (int i = 0; i < a.Rows; i++)
    for (int j = 0; j < b.Cols; j++)
        for (int k = 0; k < a.Cols; k++)
            result[i, j] += a[i, k] * b[k, j];
```

---

### 🔷 3. `SolverBase.cs` — قاعدة كل الحلّالات

**Design Pattern: Template Method Pattern**

```csharp
public abstract class SolverBase : ISolver
{
    public SolveResult Solve(Matrix matrix)
    {
        Steps.Clear();          // 1. امسح الخطوات القديمة
        return DoSolve(matrix); // 2. نادي على الـ implementation في الـ subclass
    }

    protected abstract SolveResult DoSolve(Matrix matrix); // كل subclass يكتب نفسه
```

الـ parent بيعمل الجزء المشترك (مسح الخطوات)، والـ child بيعمل اللي خاص بيه فقط.

---

### 🔷 4. `GaussJordanSolver.cs` — حل أنظمة المعادلات

**الخوارزمية: Gauss-Jordan Elimination**

**المدخل:** مصفوفة معززة `[A | b]` — الأعمدة الأولى = معاملات، آخر عمود = الحدود الحرة

```
[ 2  1 -1 | 8  ]    تمثيل:   2x + y - z = 8
[-3 -1  2 | -11]             -3x - y + 2z = -11
[-2  1  2 | -3 ]             -2x + y + 2z = -3
```

**الخطوات:**
```
1. ابحث عن أكبر عنصر في العمود (Partial Pivoting) → استقرار أفضل
2. بدّل الصفوف لو لازم
3. اضرب الصف في عكس البيفوت عشان يبقى 1
4. امسح كل الصفوف التانية في نفس العمود (Elimination)
5. كرر لكل عمود
```

**أكواد الكشف عن نوع الحل:**
```csharp
// لا حل: صف كل معاملاته أصفار لكن الحد الحر ≠ 0
// [0 0 0 | 5] → 0x + 0y + 0z = 5 → مستحيل!
if (allZeroCoeffs && !m[i, n].IsZero)
    return NoSolution;

// حلول لا نهائية: الرتبة أقل من عدد المجاهيل
if (rank < n)
    return Infinite;

// حل وحيد: كل شيء تمام
solution["x1"] = m[0, n].ToString();
```

---

### 🔷 5. `DeterminantSolver.cs` — المحدد

**الطريقة:** Row Reduction (حوّل المصفوفة لـ Upper Triangular، ثم المحدد = ضرب عناصر القطر)

```
det = 1
لكل عمود:
  ابحث عن pivot
  لو بدّلنا صفوف → اضرب det في -1 (كل تبديل يقلب الإشارة)
  اضرب det في قيمة البيفوت
  امسح العناصر تحت البيفوت
```

```csharp
// كل تبديل صفوف يعكس إشارة المحدد
m.SwapRows(col, pivotRow);
det = -det;

// المحدد = ضرب كل البيفوتات
det *= pivot;
```

---

### 🔷 6. `InverseSolver.cs` — معكوس المصفوفة

**الطريقة:** Augmented matrix `[A | I]` → يتحوّل لـ `[I | A⁻¹]`

```
نبدأ بـ:   [ 1 2 | 1 0 ]
            [ 3 4 | 0 1 ]

بعد العمليات:
            [ 1 0 | -2    1   ]
            [ 0 1 |  3/2 -1/2 ]
                   ↑
                   المعكوس هنا
```

```csharp
// بناء [A | I]
var augmented = new Matrix(n, 2 * n);
for (int i = 0; i < n; i++)
{
    for (int j = 0; j < n; j++)
        augmented[i, j] = matrix[i, j];   // A على الشمال
    augmented[i, n + i] = Fraction.One;    // I على اليمين
}
```

---

### 🔷 7. `RankSolver.cs` — الرتبة

بيحوّل المصفوفة لـ **Row Echelon Form** (شبه مثلث علوي)، والرتبة = عدد الصفوف اللي فيها pivot ≠ 0

```csharp
int rank = 0;
for (int col = 0; col < cols && rank < rows; col++)
{
    // ابحث عن pivot في هذا العمود
    // لو لقيت → rank++
}
```

---

### 🔷 8. `ISolver.cs` و `IHistoryRepository.cs` — الواجهات (Interfaces)

**ISolver — Abstraction:**
```csharp
public interface ISolver
{
    string     Name  { get; }
    SolveResult Solve(Matrix matrix);
}
```

كل solver لازم يوفّر `Name` و `Solve`. الـ `SolverService` بيشتغل مع أي `ISolver` من غير ما يهتم بالنوع — ده **Polymorphism**.

**IHistoryRepository — Repository Pattern:**
```csharp
public interface IHistoryRepository
{
    void              Save(HistoryEntry entry);
    List<HistoryEntry> LoadAll();
    void              Clear();
}
```

مش مهم لو التخزين JSON أو TXT أو Database — الـ Service بيتعامل مع الـ Interface بس.

---

### 🔷 9. `EquationParser.cs` — المحلل النصي

بيحوّل النص اللي يكتبه المستخدم لـ Matrix:

```
"2 1 -1 8"      → صف = [2/1, 1/1, -1/1, 8/1]
"-3 -1 2 -11"   → صف = [-3/1, -1/1, 2/1, -11/1]
```

**يقبل أي فاصل:**
```csharp
var tokens = row.Split(new[] { ' ', '\t', ',' }, ...);
// "1,2,3" أو "1 2 3" أو "1\t2\t3" — كلهم بيشتغلوا
```

---

### 🔷 10. `SolverService.cs` — المنسّق الرئيسي

**مبدأ Dependency Inversion:** الـ Service مش بيعمل `new JsonHistoryRepository()` — بياخده من الـ constructor:

```csharp
public class SolverService
{
    private readonly IHistoryRepository _history;

    // الـ dependency بتيجي من برّه (Dependency Injection)
    public SolverService(IHistoryRepository history)
    {
        _history = history;
    }
```

**كل عملية بتعمل:**
```
1. Parse النص → Matrix
2. اختر الـ Solver المناسب
3. شغّل Solver.Solve(matrix)
4. احفظ في History
5. ارجع النتيجة
```

---

### 🔷 11. `JsonHistoryRepository.cs` — التخزين

**Repository Pattern:** عزل التخزين عن المنطق.

```csharp
// الحفظ: قرا كل الـ entries، أضف الجديد، اكتب التاني
public void Save(HistoryEntry entry)
{
    var entries = LoadAll();
    entries.Add(entry);
    string json = JsonSerializer.Serialize(entries, _jsonOptions);
    File.WriteAllText(_filePath, json);
}
```

**مكان الملف:** `C:\Users\{username}\AppData\Roaming\LinearAlgebraSolver\history.json`

**مقاوم للتلف:** لو الملف اتعطب → ترجع قائمة فاضية بدل ما البرنامج يكرش:
```csharp
catch { return []; }   // graceful degradation
```

---

### 🔷 12. `MainWindow.xaml` — الواجهة

**Dark Theme:** ألوان من Catppuccin Mocha palette
```xml
BgDark   = #1E1E2E   <!-- الخلفية الرئيسية -->
BgPanel  = #2A2A3E   <!-- الـ panels -->
AccentBlue = #7C9EFF <!-- الأزرار والـ accents -->
```

**6 Tabs للعمليات:**
1. Gauss-Jordan — أنظمة المعادلات
2. Determinant — المحدد
3. Inverse — المعكوس
4. Transpose — الترانسبوز
5. Rank — الرتبة
6. A ⊕ B — جمع / طرح / ضرب مصفوفتين

---

### 🔷 13. `MainWindow.xaml.cs` — كود الواجهة

**Composition Root — أين يتم تركيب الـ Dependencies:**
```csharp
public MainWindow()
{
    // هنا بنقول: "استخدم JSON للحفظ"
    var repo     = new JsonHistoryRepository(historyPath);
    _history     = new HistoryService(repo);
    _solver      = new SolverService(repo);
    // لو عايز TXT بدل JSON → غير سطر واحد بس!
}
```

**كل الـ buttons بتعمل نفس الشيء:**
```csharp
private void Execute(Func<SolveResult> action)
{
    SetStatus("Computing…");          // 1. غيّر الـ status
    var result = action();            // 2. شغّل العملية
    DisplayResult(result);            // 3. اعرض النتيجة
    RefreshHistory();                 // 4. حدّث الـ History
    SetStatus(result.Summary);        // 5. اعرض الملخص
}
```

---

## 🎓 مبادئ OOP المطبّقة

| المبدأ | شرحه | أين طُبّق |
|---|---|---|
| **Encapsulation** | إخفاء البيانات الداخلية | `Matrix._data` private |
| **Abstraction** | إخفاء التفاصيل خلف interface | `ISolver`, `IHistoryRepository` |
| **Inheritance** | الوراثة | كل solver يرث من `SolverBase` |
| **Polymorphism** | التعامل مع أنواع مختلفة بنفس الكود | `SolverService` → `ISolver` |

---

## 🎨 Design Patterns المستخدمة

### Template Method Pattern
```
SolverBase.Solve()         ← الـ template (محدد في الـ parent)
    └─ DoSolve()           ← الـ step المتغير (كل child يعيد تعريفه)
```

### Strategy Pattern
```
SolverService يختار أي solver حسب العملية:
GaussJordanSolver  → لأنظمة المعادلات
DeterminantSolver  → للمحدد
InverseSolver      → للمعكوس
```

### Repository Pattern
```
IHistoryRepository (interface)
    ├── JsonHistoryRepository  → يحفظ في JSON
    └── TextHistoryRepository  → يحفظ في TXT
```

### Dependency Injection
```csharp
// بدل:
var service = new SolverService(new JsonHistoryRepository(...));

// الـ service لا يعرف نوع الـ repo — يستقبله من برّه
public SolverService(IHistoryRepository history) { ... }
```

---

## 🔄 تدفق البيانات — مثال كامل

**السيناريو:** المستخدم يحل نظام معادلات

```
1. يكتب في TextBox:
   "2 1 -1 8
    -3 -1 2 -11
    -2 1 2 -3"

2. يضغط ▶ Solve

3. MainWindow.BtnSolveGauss_Click()
   └─ Execute(() => _solver.SolveLinearSystem(text))

4. SolverService.SolveLinearSystem(text)
   ├─ EquationParser.ParseMatrix(text) → Matrix 3×4
   ├─ new GaussJordanSolver()
   ├─ solver.Solve(matrix) → SolveResult
   └─ _history.Save(new HistoryEntry { ... })

5. GaussJordanSolver.DoSolve(matrix)
   ├─ Step 1: Starting augmented matrix
   ├─ Step 2: Swap R1 ↔ R2
   ├─ Step 3: R1 = R1 / -3
   ├─ ... (كل خطوة محفوظة)
   └─ Solution: x1=2, x2=3, x3=-1

6. MainWindow.DisplayResult(result)
   └─ StepsOutput.Text = كل الخطوات مع المصفوفات

7. HistoryList يتحدث — يظهر العملية الجديدة
```

---

## 🧪 شرح الاختبارات (Tests)

### FractionTests — 18 اختبار
```csharp
// هل 1/2 + 1/3 = 5/6؟
[Theory]
[InlineData(1, 2, 1, 3, 5, 6)]
public void Addition_IsCorrect(...)
{ Assert.Equal(new Fraction(5,6), new Fraction(1,2) + new Fraction(1,3)); }

// هل بيرمي exception لو المقام صفر؟
[Fact]
public void Constructor_ThrowsOnZeroDenominator()
{ Assert.Throws<DivideByZeroException>(() => new Fraction(1, 0)); }
```

### MatrixTests — 9 اختبارات
```csharp
// هل I × A = A؟
[Fact]
public void Multiply_Identity_ReturnsOriginal()
{
    var result = a * Matrix.Identity(2);
    Assert.Equal(a[0, 0], result[0, 0]);
}
```

### SolverTests — 14 اختبار
```csharp
// هل يحل بشكل صحيح؟
[Fact]
public void GaussJordan_UniqueSolution_IsCorrect()
{
    // 2x + y - z = 8, -3x - y + 2z = -11, -2x + y + 2z = -3
    // Expected: x=2, y=3, z=-1
    Assert.Equal("2",  result.Solution["x1"]);
    Assert.Equal("3",  result.Solution["x2"]);
    Assert.Equal("-1", result.Solution["x3"]);
}
```

---

## 💡 نقاط قوة المشروع

1. **دقة رياضية 100%** — لا يوجد خطأ عشري، كل شيء كسور
2. **قابلية التوسع** — أضف solver جديد بملف واحد فقط
3. **History دائم** — يحفظ على الـ disk، يبقى بعد إغلاق البرنامج
4. **اختبارات شاملة** — 41 اختبار كلهم ناجحون ✅
5. **كود نظيف** — Single Responsibility، كل class مهمة واحدة

---

## 🛠️ كيف تُشغّل المشروع؟

```powershell
# تشغيل الـ UI
dotnet run --project LinearAlgebra.UI

# تشغيل الاختبارات
dotnet test LinearAlgebra.Tests

# Build release EXE
dotnet publish LinearAlgebra.UI -c Release -o publish/
# الـ EXE في: publish/LinearAlgebraSolver.exe
```
