﻿// Copyright (c) The Avalonia Project. All rights reserved.
// Licensed under the MIT license. See licence.md file in the project root for full license information.

using System.Linq;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Platform;
using Avalonia.Rendering;
using Avalonia.UnitTests;
using Avalonia.VisualTree;
using Moq;
using Xunit;
using System;
using Avalonia.Controls.Shapes;

namespace Avalonia.Visuals.UnitTests.VisualTree
{
    public class VisualExtensionsTests_GetVisualsAt
    {
        [Fact]
        public void GetVisualsAt_Should_Find_Controls_At_Point()
        {
            using (TestApplication())
            {
                var container = new TestRoot
                {
                    Width = 200,
                    Height = 200,
                    Child = new Border
                    {
                        Width = 100,
                        Height = 100,
                        Background = Brushes.Red,
                        HorizontalAlignment = HorizontalAlignment.Center,
                        VerticalAlignment = VerticalAlignment.Center
                    }
                };

                container.Measure(Size.Infinity);
                container.Arrange(new Rect(container.DesiredSize));

                var result = container.GetVisualsAt(new Point(100, 100));

                Assert.Equal(new[] { container.Child }, result);
            }
        }

        [Fact]
        public void GetVisualsAt_Should_Not_Find_Empty_Controls_At_Point()
        {
            using (TestApplication())
            {
                var container = new TestRoot
                {
                    Width = 200,
                    Height = 200,
                    Child = new Border
                    {
                        Width = 100,
                        Height = 100,
                        HorizontalAlignment = HorizontalAlignment.Center,
                        VerticalAlignment = VerticalAlignment.Center
                    }
                };

                container.Measure(Size.Infinity);
                container.Arrange(new Rect(container.DesiredSize));

                var result = container.GetVisualsAt(new Point(100, 100));

                Assert.Empty(result);
            }
        }

        [Fact]
        public void GetVisualsAt_Should_Not_Find_Invisible_Controls_At_Point()
        {
            using (TestApplication())
            {
                Border visible;
                var container = new TestRoot
                {
                    Width = 200,
                    Height = 200,
                    Child = new Border
                    {
                        Width = 100,
                        Height = 100,
                        Background = Brushes.Red,
                        HorizontalAlignment = HorizontalAlignment.Center,
                        VerticalAlignment = VerticalAlignment.Center,
                        IsVisible = false,
                        Child = visible = new Border
                        {
                            Background = Brushes.Red,
                            HorizontalAlignment = HorizontalAlignment.Stretch,
                            VerticalAlignment = VerticalAlignment.Stretch,
                        }
                    }
                };

                container.Measure(Size.Infinity);
                container.Arrange(new Rect(container.DesiredSize));

                var result = container.GetVisualsAt(new Point(100, 100));

                Assert.Empty(result);
            }
        }

        [Fact]
        public void GetVisualsAt_Should_Not_Find_Control_Outside_Point()
        {
            using (TestApplication())
            {
                var container = new TestRoot
                {
                    Width = 200,
                    Height = 200,
                    Child = new Border
                    {
                        Width = 100,
                        Height = 100,
                        Background = Brushes.Red,
                        HorizontalAlignment = HorizontalAlignment.Center,
                        VerticalAlignment = VerticalAlignment.Center
                    }
                };

                container.Measure(Size.Infinity);
                container.Arrange(new Rect(container.DesiredSize));

                var result = container.GetVisualsAt(new Point(10, 10));

                Assert.Empty(result);
            }
        }

        [Fact]
        public void GetVisualsAt_Should_Return_Top_Controls_First()
        {
            using (TestApplication())
            {
                Panel container;
                var root = new TestRoot
                {
                    Child = container = new Panel
                    {
                        Width = 200,
                        Height = 200,
                        Children = new Controls.Controls
                        {
                            new Border
                            {
                                Width = 100,
                                Height = 100,
                                Background = Brushes.Red,
                                HorizontalAlignment = HorizontalAlignment.Center,
                                VerticalAlignment = VerticalAlignment.Center
                            },
                            new Border
                            {
                                Width = 50,
                                Height = 50,
                                Background = Brushes.Red,
                                HorizontalAlignment = HorizontalAlignment.Center,
                                VerticalAlignment = VerticalAlignment.Center
                            }
                        }
                    }
                };

                root.Measure(Size.Infinity);
                root.Arrange(new Rect(container.DesiredSize));

                var result = container.GetVisualsAt(new Point(100, 100));

                Assert.Equal(new[] { container.Children[1], container.Children[0] }, result);
            }
        }

        [Fact]
        public void GetVisualsAt_Should_Return_Top_Controls_First_With_ZIndex()
        {
            using (TestApplication())
            {
                Panel container;
                var root = new TestRoot
                {
                    Child = container = new Panel
                    {
                        Width = 200,
                        Height = 200,
                        Children = new Controls.Controls
                        {
                            new Border
                            {
                                Width = 100,
                                Height = 100,
                                ZIndex = 1,
                                Background = Brushes.Red,
                                HorizontalAlignment = HorizontalAlignment.Center,
                                VerticalAlignment = VerticalAlignment.Center
                            },
                            new Border
                            {
                                Width = 50,
                                Height = 50,
                                Background = Brushes.Red,
                                HorizontalAlignment = HorizontalAlignment.Center,
                                VerticalAlignment = VerticalAlignment.Center
                            },
                            new Border
                            {
                                Width = 75,
                                Height = 75,
                                ZIndex = 2,
                                Background = Brushes.Red,
                                HorizontalAlignment = HorizontalAlignment.Center,
                                VerticalAlignment = VerticalAlignment.Center
                            }
                        }
                    }
                };

                root.Measure(Size.Infinity);
                root.Arrange(new Rect(container.DesiredSize));

                var result = container.GetVisualsAt(new Point(100, 100));

                Assert.Equal(new[] { container.Children[2], container.Children[0], container.Children[1] }, result);
            }
        }

        [Fact]
        public void GetVisualsAt_Should_Find_Control_Translated_Outside_Parent_Bounds()
        {
            using (TestApplication())
            {
                Border target;
                Panel container;
                var root = new TestRoot
                {
                    Child = container = new Panel
                    {
                        Width = 200,
                        Height = 200,
                        Background = Brushes.Red,
                        ClipToBounds = false,
                        Children = new Controls.Controls
                        {
                            new Border
                            {
                                Width = 100,
                                Height = 100,
                                ZIndex = 1,
                                Background = Brushes.Red,
                                HorizontalAlignment = HorizontalAlignment.Left,
                                VerticalAlignment = VerticalAlignment.Top,
                                Child = target = new Border
                                {
                                    Width = 50,
                                    Height = 50,
                                    Background = Brushes.Red,
                                    HorizontalAlignment = HorizontalAlignment.Left,
                                    VerticalAlignment = VerticalAlignment.Top,
                                    RenderTransform = new TranslateTransform(110, 110),
                                }
                            },
                        }
                    }
                };

                container.Measure(Size.Infinity);
                container.Arrange(new Rect(container.DesiredSize));

                var result = container.GetVisualsAt(new Point(120, 120));

                Assert.Equal(new IVisual[] { target, container }, result);
            }
        }

        [Fact]
        public void GetVisualsAt_Should_Not_Find_Control_Outside_Parent_Bounds_When_Clipped()
        {
            using (TestApplication())
            {
                Border target;
                Panel container;
                var root = new TestRoot
                {
                    Child = container = new Panel
                    {
                        Width = 100,
                        Height = 200,
                        Background = Brushes.Red,
                        Children = new Controls.Controls
                        {
                            new Panel()
                            {
                                Width = 100,
                                Height = 100,
                                Background = Brushes.Red,
                                Margin = new Thickness(0, 100, 0, 0),
                                ClipToBounds = true,
                                Children = new Controls.Controls
                                {
                                    (target = new Border()
                                    {
                                        Width = 100,
                                        Height = 100,
                                        Background = Brushes.Red,
                                        Margin = new Thickness(0, -100, 0, 0)
                                    })
                                }
                            }
                        }
                    }
                };

                root.Measure(Size.Infinity);
                root.Arrange(new Rect(container.DesiredSize));

                var result = container.GetVisualsAt(new Point(50, 50));

                Assert.Equal(new[] { container }, result);
            }
        }

        [Fact]
        public void GetVisualsAt_Should_Not_Find_Control_Outside_Scroll_Viewport()
        {
            using (TestApplication())
            {
                Border target;
                Border item1;
                Border item2;
                ScrollContentPresenter scroll;
                Panel container;
                var root = new TestRoot
                {
                    Child = container = new Panel
                    {
                        Width = 100,
                        Height = 200,
                        Background = Brushes.Red,
                        Children = new Controls.Controls
                        {
                            (target = new Border()
                            {
                                Name = "b1",
                                Width = 100,
                                Height = 100,
                                Background = Brushes.Red,
                            }),
                            new Border()
                            {
                                Name = "b2",
                                Width = 100,
                                Height = 100,
                                Background = Brushes.Red,
                                Margin = new Thickness(0, 100, 0, 0),
                                Child = scroll = new ScrollContentPresenter()
                                {
                                    Content = new StackPanel()
                                    {
                                        Children = new Controls.Controls
                                        {
                                            (item1 = new Border()
                                            {
                                                Name = "b3",
                                                Width = 100,
                                                Height = 100,
                                                Background = Brushes.Red,
                                            }),
                                            (item2 = new Border()
                                            {
                                                Name = "b4",
                                                Width = 100,
                                                Height = 100,
                                                Background = Brushes.Red,
                                            }),
                                        }
                                    }
                                }
                            }
                        }
                    }
                };

                scroll.UpdateChild();

                root.Measure(Size.Infinity);
                root.Arrange(new Rect(container.DesiredSize));

                var result = container.GetVisualsAt(new Point(50, 150)).First();

                Assert.Equal(item1, result);

                result = container.GetVisualsAt(new Point(50, 50)).First();

                Assert.Equal(target, result);

                scroll.Offset = new Vector(0, 100);

                // We don't have LayoutManager set up so do the layout pass manually.
                scroll.Parent.InvalidateArrange();
                container.InvalidateArrange();
                container.Arrange(new Rect(container.DesiredSize));

                result = container.GetVisualsAt(new Point(50, 150)).First();
                Assert.Equal(item2, result);

                result = container.GetVisualsAt(new Point(50, 50)).First();
                Assert.Equal(target, result);
            }
        }

        [Fact]
        public void GetVisualsAt_Should_Not_Find_Path_When_Outside_Fill()
        {
            using (TestApplication())
            {
                Path path;
                var container = new TestRoot
                {
                    Width = 200,
                    Height = 200,
                    Child = path = new Path
                    {
                        Width = 200,
                        Height = 200,
                        Fill = Brushes.Red,
                        Data = StreamGeometry.Parse("M100,0 L0,100 100,100")
                    }
                };

                container.Measure(Size.Infinity);
                container.Arrange(new Rect(container.DesiredSize));

                var context = new DrawingContext(Mock.Of<IDrawingContextImpl>());

                var result = container.GetVisualsAt(new Point(100, 100));
                Assert.Equal(new[] { path }, result);

                result = container.GetVisualsAt(new Point(10, 10));
                Assert.Empty(result);
            }
        }

        [Fact]
        public void GetVisualsAt_Should_Respect_Geometry_Clip()
        {
            using (TestApplication())
            {
                Border border;
                Canvas canvas;
                var container = new TestRoot
                {
                    Width = 400,
                    Height = 400,
                    Child = border = new Border
                    {
                        Background = Brushes.Red,
                        Clip = StreamGeometry.Parse("M100,0 L0,100 100,100"),
                        Width = 200,
                        Height = 200,
                        Child = canvas = new Canvas
                        {
                            Background = Brushes.Yellow,
                            Margin = new Thickness(10),
                        }
                    }
                };

                container.Measure(Size.Infinity);
                container.Arrange(new Rect(container.DesiredSize));
                Assert.Equal(new Rect(100, 100, 200, 200), border.Bounds);

                var context = new DrawingContext(Mock.Of<IDrawingContextImpl>());

                var result = container.GetVisualsAt(new Point(200, 200));
                Assert.Equal(new IVisual[] { canvas, border }, result);

                result = container.GetVisualsAt(new Point(110, 110));
                Assert.Empty(result);
            }
        }

        private IDisposable TestApplication()
        {
            return UnitTestApplication.Start(
                new TestServices(
                    renderInterface: new MockPlatformRenderInterface(),
                    renderer: (root, loop) => new ImmediateRenderer(root)));
        }
    }
}
