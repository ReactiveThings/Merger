using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace ReactiveThings.Merger.UnitTests
{
    public class UsageExamples
    {
        [Fact]
        public async Task MergeUsingExtensionMethod()
        {
            var source = new List<Model>() {
                new Model{Id = null},
                new Model{Id = 1}
            };
            var target = new List<Entity>() {
                new Entity{Id = 1},
                new Entity{Id = 2}
            };

            await target.MergeUsing(source)
                .On(model => model.Id, entity => entity.Id)
                .WhenMatched((model, entity) => entity.Property = model.Property)
                .WhenNotMatched((model) => target.Add(new Entity { Property = model.Property }))
                .WhenNotMatchedBySource(entity => target.Remove(entity))
                .ExecuteAsync();
        }

        [Fact]
        public async Task MergeUsingStaticMethod()
        {
            var source = new List<Model>() {
                new Model{Id = null},
                new Model{Id = 1}
            };
            var target = new List<Entity>() {
                new Entity{Id = 1},
                new Entity{Id = 2}
            };

            await Merger.Merge(target).Using(source).On(model => model.Id, entity => entity.Id)
                .WhenMatched((model, entity) => entity.Property = model.Property)
                .WhenNotMatched((model) => target.Add(new Entity { Property = model.Property }))
                .WhenNotMatchedBySource(entity => target.Remove(entity))
                .ExecuteAsync();
        }

        [Fact]
        public async Task MergeUsingAsyncMethod()
        {
            var source = new List<Model>() {
                new Model{Id = null},
                new Model{Id = 1}
            };
            var target = new List<Entity>() {
                new Entity{Id = 1},
                new Entity{Id = 2}
            };

            await Merger.Merge(target).Using(source).On(model => model.Id, entity => entity.Id)
                .WhenMatched(async (model, entity) => {
                    await AsyncMethod();
                    entity.Property = model.Property;
                })
                .WhenNotMatched(async (model) => {
                    await AsyncMethod();
                    target.Add(new Entity { Property = model.Property });
                })
                .WhenNotMatchedBySource(async entity => {
                    await AsyncMethod();
                    target.Remove(entity);
                 })
                .ExecuteAsync();
        }

        private Task AsyncMethod()
        {
            return Task.CompletedTask;
        }
    }
}
