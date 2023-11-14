using Bunit;
using DxTestProject.Pages;
using System;
using Xunit;
using Microsoft.Extensions.DependencyInjection;
using DevExpress.Blazor;

namespace DevExpressBunit.Test {
    public class GridApplyFilter : IDisposable {
        public TestContext Context { get; }
        public GridApplyFilter() {
            Context = new TestContext();
            Context.Services.AddOptions();
            Context.AddDevExpressBlazorTesting();
        }
        [Fact]
        public void CheckIfNodeClickAppliesFilter() {
            var cut = Context.RenderComponent<Grid_ApplyFilter>();
            var treeview = cut.FindComponent<DxTreeView>().Instance;
            var grid = cut.FindComponent<DxGrid>().Instance;
            cut.InvokeAsync(() => treeview.SelectNode(x => x.Text == "Filter by date"));
            Assert.Equal(1, grid.GetVisibleRowCount());
            cut.InvokeAsync(() => treeview.SelectNode(x => x.Text == "Filter by temperature"));
            Assert.Equal(3, grid.GetVisibleRowCount());
            cut.InvokeAsync(() => treeview.SelectNode(x => x.Text == "Filter by precipitation"));
            Assert.Equal(6, grid.GetVisibleRowCount());
        }
        public void Dispose() {
            Context.Dispose();
        }
    }
}
