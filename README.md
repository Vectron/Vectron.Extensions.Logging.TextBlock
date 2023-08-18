# Vectron.Extensions.Logging.TextBlock
[![License: MIT](https://img.shields.io/badge/License-MIT-green.svg)](https://github.com/Vectron/Vectron.Extensions.Logging.TextBlock/blob/main/LICENSE.txt)
[![Build status](https://github.com/Vectron/Vectron.Extensions.Logging.TextBlock/actions/workflows/BuildTestDeploy.yml/badge.svg)](https://github.com/Vectron/Vectron.Extensions.Logging.TextBlock/actions)
[![NuGet](https://img.shields.io/nuget/v/Vectron.Extensions.Logging.TextBlock.svg)](https://www.nuget.org/packages/Vectron.Extensions.Logging.TextBlock)

Vectron.Extensions.Logging.TextBlock provides a WPF TextBlock logger provider implementation for Microsoft.Extensions.Logging.

To mark a TextBlock as the logger target you can use a behavior.
In the next example the TextBlock will be used to display the logs.
```xml
<Window
    x:Class="LogExample.Wpf.MainWindow"
    xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
    xmlns:d="http://schemas.microsoft.com/expression/blend/2008"
    xmlns:mc="http://schemas.openxmlformats.org/markup-compatibility/2006"
    xmlns:textblockLogging="clr-namespace:Vectron.Extensions.Logging.TextBlock;assembly=Vectron.Extensions.Logging.TextBlock"
    Title="Logging example"
    mc:Ignorable="d">
    <DockPanel>
        <ScrollViewer VerticalScrollBarVisibility="Auto">
            <TextBlock
                textblockLogging:TextblockLoggerBehavior.LoggerTarget="True"
                DockPanel.Dock="Bottom" />
        </ScrollViewer>
    </DockPanel>
</Window>
```


## Authors
- [@Vectron](https://www.github.com/Vectron)
