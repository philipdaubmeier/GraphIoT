using PhilipDaubmeier.GraphIoT.Core.ViewModel;
using PhilipDaubmeier.GraphIoT.Graphite.Model;
using PhilipDaubmeier.GraphIoT.Graphite.Parser.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace GraphIoT.Graphite.Tests
{
    public class QueryParserTest
    {
        private class MockGraphViewModel : GraphViewModel
        {
            public MockGraphViewModel() : base()
            {
                Begin = new DateTime(2020, 1, 1, 12, 0, 0, DateTimeKind.Utc);
                Spacing = new TimeSpan(0, 1, 0);
                Name = "unit.test";
                Key = Name;
                Points = new List<dynamic>() { 0.01d, 0.08d, 0.2d, 0.7d, 0.9d, 1.0d, 1.1d, 0.8d, 0.3d, -0.2d };
            }
        }

        private class MockGraphCollectionViewModel : GraphCollectionViewModelBase
        {
            public override string Key => "mock";
            protected override void InvalidateData() { }
            public override int GraphCount() => 1;
            public override IEnumerable<string> GraphKeys() => Graphs().Select(g => g.Key);
            public override GraphViewModel Graph(int index) => new MockGraphViewModel();
            public override IEnumerable<GraphViewModel> Graphs() => new[] { Graph(0) };
        }

        [Theory]
        [InlineData("movingWindow(mock.unit.test,'3min','avg')")]
        [InlineData("movingWindow(mock.unit.test, '3min', 'avg')")]
        [InlineData("      movingWindow(mock.unit.test,'3min','avg')    ")]
        [InlineData("  \t \r\n  movingWindow \n\n  ( \n  mock.unit.test    , \r\n '3min'   ,  \r\n  'avg'  )  \t \r\n  ")]
        public void ParserWhitepaceInvarianceTest(string query)
        {
            var dataSource = new GraphDataSource(new[] { new MockGraphCollectionViewModel() });
            var parser = new Parser() { DataSource = dataSource };
            var ast = parser.Parse(query);

            Assert.IsType<MovingWindowFunctionExpression>(ast);
            Assert.IsType<DerivedGraphTransform>(ast.Graphs.First());
            Assert.Equal("unit.test", ((DerivedGraphTransform)ast.Graphs.First()).Name);
            Assert.Equal(0.259999d, ((DerivedGraphTransform)ast.Graphs.First()).Points.Skip(3).First()!.Value, 3);
            Assert.Equal(0.533333d, ((DerivedGraphTransform)ast.Graphs.First()).Points.Skip(4).First()!.Value, 3);
            Assert.Equal(0.866666d, ((DerivedGraphTransform)ast.Graphs.First()).Points.Skip(5).First()!.Value, 3);
        }
    }
}