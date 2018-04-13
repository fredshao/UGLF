# UGLF
Unity Game Logic Framework

此工程的目标是建立一个清晰、方便使用、所见即所得，可以节省大量开发时间的Unity逻辑层框架

**使用:**

直接将工程编译成DLL，将生成的UGLF.dll放到Unity工程的Plugins目录即可，也可以直接将Src目录放到工程中，直接使用源代码

**注意:**

1. ReferenceDlls是框架编写时所引用的Unity的一些DLL，这里使用的Unity版本在ReferenceDlls的"说明.txt"文件中标注
2. 为了方便使用，框架不使用任何命名空间，但是为了避免和使用者原有代码产生冲突，每一个类会添加前缀 "UGLF"

**资源管理规范:**

1. ​