# 扫雷_HOS

## 简介

以炉石UI为基地制作的扫雷小游戏，支持自定义游戏

优点是单文件，小体积，适合随手打开玩

## 玩法

同普通扫雷，鼠标左键点击打开方块，右键单击标记方块，双击方块为快速打开

此外加入了**嗅探猫**功能，点击后鼠标光标变为瞄准镜状态，你的下一次点击如果是空白块，则会正常打开，如果是雷块，则会进行标记，**嗅探猫每局游戏只能用一次**

## 构成

* 左侧：游戏区

  > 中间部分为方块放置区
  >
  > 右下角为开启/关闭嗅探猫

  游戏完成后点击游戏区任意处可以以当前设置重新开始游戏

* 右侧：设置区

  > 右上角三个滑杆分别控制：行数、列数、雷数
  >
  > 滑杆下方为游戏统计
  >
  > 右侧中部的三个圆形按钮：快速开始简单、普通、专家模式
  >
  > 右侧底部的大圆按钮为开始按钮：
  >
  > * 左键：以当前设置开始游戏
  > * 右键：开始随机模式

  设置去左侧有一个带箭头的按钮，点击该按钮可以显示/隐藏设置区

## 快捷键：

添加了一些简单的快捷键支持

* <kbd>F1</kbd>: 开始简单模式
* <kbd>F2</kbd>：开始普通模式
* <kbd>F3</kbd>：开始专家模式
* <kbd>F5</kbd>：随机模式
* <kbd>空格</kbd>：以当前设置开始游戏

## 设定限制

行数：6 ~ 18

列数：6 ~ 30

雷数：4 ~ (行数x列数)/4

