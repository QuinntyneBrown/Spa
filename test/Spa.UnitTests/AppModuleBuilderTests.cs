using System.Collections.Generic;
using Xunit;

namespace Spa.UnitTests
{
    public class AppModuleBuilderTests
    {
        [Fact]
        public void Basic()
        {
            var expected = new List<string>()
            {
                "import { BrowserModule } from '@angular/platform-browser';",
                "import { NgModule } from '@angular/core';",
                "import { AppRoutingModule } from './app-routing.module';",
                "import { AppComponent } from './app.component';",
                "",
                "@NgModule({"
            }.ToArray();
        }
    }
}
