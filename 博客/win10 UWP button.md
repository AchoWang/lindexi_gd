button�кܶ��wpfһ�������Կ�������ǳ��WPF��
button�����������ԣ�ʹ����Դ
��Դ����д��ҳ��

```
    <Page.Resources>
        
    </Page.Resources>
```

���а�ťʹ��ͬ��ʽ

```
    <Page.Resources>
        <Style TargetType="Button">
            
        </Style>
    </Page.Resources>
```

��ť��������`<Setter Property="����" Value="ֵ"/>`

��ť�ı���

```
    <Page.Resources>
        <Style TargetType="Button">
            <Setter Property="Background" Value="White"/>
        </Style>
    </Page.Resources>
```

ָ��һ����ʽ��key

```
    <Page.Resources>
        <Style TargetType="Button">
            <Setter Property="Background" Value="White"/>
            <Setter Property="Width" Value="100"/>
            <Setter Property="Height" Value="100"/>
        </Style>
        <Style  x:Key="button" TargetType="Button">
            <Setter Property="Background" Value="White"/>
            <Setter Property="Width" Value="50"/>
            <Setter Property="Height" Value="50"/>
        </Style>
    </Page.Resources>
```

```
         <Button Content="Ĭ��"/>
         <Button Style="{StaticResource button}" Content="ȷ��"/>
```

![����дͼƬ����](image/20151211154753136.jpg)

�ƶ���button��ʾ����

��װ���ر��ƶ����Ѻ���ʾ�Ѻ�
�ο���http://blog.csdn.net/lindexi_gd/article/details/50166161

```
                        <Button Click="souhu_Click" ToolTipService.ToolTip="�Ѻ���Ƶ" Padding="0" >
                            <Button.Content>
                                <Grid>
                                    <Grid.RowDefinitions>
                                        <RowDefinition Height="auto"/>
                                        <RowDefinition Height="auto"/>
                                    </Grid.RowDefinitions>
                                    <Image Source="ms-appx:///Assets/�Ѻ�.png" Grid.Row="0" ScrollViewer.VerticalScrollBarVisibility="Disabled" />
                                    <TextBlock Text="�Ѻ���Ƶ" Grid.Row="1" HorizontalAlignment="Center" />
                                </Grid>
                            </Button.Content>
                        </Button>
```

![����дͼƬ����](image/20151211161126290.jpg)

��ʾͼƬ

```
                        <Button Click="souhu_Click" Padding="0" >
                            <Button.Content>
                                <Grid>
                                    <Grid.RowDefinitions>
                                        <RowDefinition Height="auto"/>
                                        <RowDefinition Height="auto"/>
                                    </Grid.RowDefinitions>
                                    <Image Source="ms-appx:///Assets/�Ѻ�.png" Grid.Row="0" ScrollViewer.VerticalScrollBarVisibility="Disabled" />
                                    <TextBlock Text="�Ѻ���Ƶ" Grid.Row="1" HorizontalAlignment="Center" />
                                </Grid>
                            </Button.Content>
                            <ToolTipService.ToolTip>
                                <Image Height="50" Width="50" Source="ms-appx:///Assets/�Ѻ�.png"/>
                            </ToolTipService.ToolTip>
                        </Button>
```

