using Bunit;
using DxTestProject.Pages;
using System;
using System.Threading.Tasks;
using Xunit;
using Microsoft.Extensions.DependencyInjection;
using DevExpress.Blazor;

namespace DevExpressBunit.Test {
    public class GridApplyFilter : BunitContext {
        public GridApplyFilter() {
            Services.AddOptions();
            this.AddDevExpressBlazorTesting();
        }
        [Fact]
        public async Task CheckIfNodeClickAppliesFilter() {
            var cut = Render<Grid_ApplyFilter>();
            var treeview = cut.FindComponent<DxTreeView>().Instance;
            var grid = cut.FindComponent<DxGrid>().Instance;
            await cut.InvokeAsync(() => treeview.SelectNode(x => x.Text == "Filter by date"));
            Assert.Equal(1, grid.GetVisibleRowCount());
            await cut.InvokeAsync(() => treeview.SelectNode(x => x.Text == "Filter by temperature"));
            Assert.Equal(3, grid.GetVisibleRowCount());
            await cut.InvokeAsync(() => treeview.SelectNode(x => x.Text == "Filter by precipitation"));
            Assert.Equal(6, grid.GetVisibleRowCount());
        }
    }
}
