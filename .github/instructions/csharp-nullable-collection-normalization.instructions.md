---
description: "当修改 aspnet-core/src 与 aspnet-core/test 下启用可空引用类型的 C# 代码时使用。优先采用安全的集合类型与字符串可空归一化策略。"
name: "C# 可空集合归一化"
applyTo: "aspnet-core/src/**/*.cs, aspnet-core/test/**/*.cs"
---
# C# 可空集合归一化

- 在可空上下文下，优先避免将 `List<string?>` 直接赋值给 `List<string>`。
- 优先在物化为非空强类型集合之前，先对可空字符串序列做归一化。
- 优先在查询边界通过 `?? string.Empty` 或 `Where(x => x != null)` 后再显式转换。
- 优先保持方法边界两侧的集合类型一致，避免隐式可空漂移。
- 当返回 `List<string>` 时，优先从构造过程保证元素非空。
