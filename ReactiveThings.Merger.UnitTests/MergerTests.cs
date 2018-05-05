using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace ReactiveThings.Merger.UnitTests
{
    public class Model
    {
        public int? Id { get; set; }
        public int Property { get; set; }
    }

    public class Entity
    {
        public int Id { get; set; }

        public int Property { get; set; }
    }

    public class MergerTests
    {
        [Fact]
        public async Task MergeMatchedIsCalledWhenThereIsMatchedItemInBothCollections()
        {
            var source = new List<Model>() {
                new Model{Id = 1}
            };
            var target = new List<Entity>() {
                new Entity{Id = 1}
            };

            var called = false;

            await CreateMerger(source, target)
                .WhenMatched((model, entity) => called = true)
                .WhenNotMatched((model) => Assert.True(false, "WhenNotMatched called"))
                .WhenNotMatchedBySource(entity => Assert.True(false, "WhenNotMatchedBySource called"))
                .ExecuteAsync();

            Assert.True(called);
        }

        [Fact]
        public async Task ExecuteAsyncThrowsExceptionWhenOneSourceRowMatchesManyTargetRows()
        {
            var source = new List<Model>() {
                new Model{Id = 1}
            };
            var target = new List<Entity>() {
                new Entity{Id = 1},
                new Entity{Id = 1}
            };

            var merger = CreateMerger(source, target)
                .WhenMatched((model, entity) => { })
                .WhenNotMatched((model) => Assert.True(false, "WhenNotMatched called"))
                .WhenNotMatchedBySource(entity => Assert.True(false, "WhenNotMatchedBySource called"));

            await Assert.ThrowsAsync<ArgumentException>(async () => { await merger.ExecuteAsync(); });
        }

        [Fact]
        public async Task ExecuteAsyncThrowsExceptionWhenOneTargetRowMatchesManySourceRows()
        {
            var source = new List<Model>() {
                new Model{Id = 1},
                new Model{Id = 1}
            };
            var target = new List<Entity>() {
                new Entity{Id = 1}
            };

            var merger = CreateMerger(source, target)
                .WhenMatched((model, entity) => { })
                .WhenNotMatched((model) => Assert.True(false, "WhenNotMatched called"))
                .WhenNotMatchedBySource(entity => Assert.True(false, "WhenNotMatchedBySource called"));

            await Assert.ThrowsAsync<ArgumentException>(async () => { await merger.ExecuteAsync(); });
        }

        [Fact]
        public async Task MergeNotMatchedIsCalledWhenThereIsNoMatchedItemInTargetCollection()
        {
            var source = new List<Model>(){
                new Model{Id = 1}
            };
            var target = new List<Entity>();

            var called = false;

            await CreateMerger(source, target)
                .WhenMatched((model, entity) => Assert.True(false, "WhenMatched called"))
                .WhenNotMatched((model) => called = true)
                .WhenNotMatchedBySource(entity => Assert.True(false, "WhenNotMatchedBySource called"))
                .ExecuteAsync();

            Assert.True(called);
        }

        [Fact]
        public async Task MergeNotMatchedBySourceIsCalledWhenThereIsNoMatchedItemInSourceCollection()
        {
            var source = new List<Model>();
            var target = new List<Entity>() {
                new Entity{Id = 1}
            };

            var called = false;

            await CreateMerger(source, target)
                .WhenMatched((model, entity) => Assert.True(false, "WhenMatched called"))
                .WhenNotMatched((model) => Assert.True(false, "WhenNotMatched called"))
                .WhenNotMatchedBySource(entity => called = true)
                .ExecuteAsync();

            Assert.True(called);
        }

        [Fact]
        public async Task WhenKeyInSourceItemIsNullCallNotMatched()
        {
            var source = new List<Model>() {
                new Model{Id = null},

            };
            var target = new List<Entity>() {
                new Entity{Id = 1}
            };
            var whenNotMatchedCalled = false;
            var whenNotMatchedBySourceCalled = false;
            await CreateMerger(source, target)
                .WhenMatched((model, entity) => Assert.True(false, "WhenMatched called"))
                .WhenNotMatched((model) => whenNotMatchedCalled = true)
                .WhenNotMatchedBySource(entity => whenNotMatchedBySourceCalled = true)
                .ExecuteAsync();
            Assert.True(whenNotMatchedCalled && whenNotMatchedBySourceCalled);
        }

        [Fact]
        public async Task ExecuteAsyncShouldWorkWithoutProvidedMergeActions()
        {
            var source = new List<Model>() {
                new Model{Id = null},
                new Model{Id = 1}
            };
            var target = new List<Entity>() {
                new Entity{Id = 1},
                new Entity{Id = 2}
            };

            await CreateMerger(source, target)
                .ExecuteAsync();

        }

        private IMergerBuilder<Model, Entity> CreateMerger(List<Model> source, List<Entity> target)
        {
            return Merger.Merge(target)
                            .Using(source)
                            .On(model => model.Id, entity => entity.Id);
        }
    }
}
