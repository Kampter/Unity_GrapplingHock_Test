#### 总结只狼抓钩
1. 同一时间只有一个可以抓取的目标物
2. 距离过远不会出现
3. 距离到一定范围内会出现UI但是灰色不可交互
4. 距离再次接近会变成绿色可以交互
5. 可以在行走或者跳跃中使用抓钩
6. 在抓钩动画中可以再次抓取新的目标
7. 不同关卡抓钩的规则略有不同


#### 操作方式：
1. WASD控制方向
2. Tab可以切换目标视角
3. 鼠标右键可以将人物向目标位置抓钩


#### 额外使用的插件:
1. Cinemachine
2. Input System
插件都是Unity官方提供的，应该不属于第三方插件范畴。

   
#### 目前存在的问题:
1. 缺少墙体背后的判断
2. 抓钩缺少freeze state
3. 缺少抓钩动画
4. 实现方式是向目标位置添加速度，不能保证一定能将自身拉动到目标位置
5. LineRenderer 位置更新问题，以及出现时间过短
