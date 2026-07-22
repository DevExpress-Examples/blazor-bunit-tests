using Bunit;
using DxTestProject.Pages;
using System;
using Xunit;
using Microsoft.Extensions.DependencyInjection;
using DevExpress.Blazor;

namespace DevExpressBunit.Test {
    public class GridApplyFilter : IDisposable {
        public BunitContext Context { get; }
        public GridApplyFilter() {
            Context = new BunitContext();
            Context.Services.AddOptions();
            Context.AddDevExpressBlazorTesting();
        }
        [Fact]
        public void CheckIfNodeClickAppliesFilter() {
            var cut = Context.Render<Grid_ApplyFilter>();
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
