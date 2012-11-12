﻿using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Glimpse.Core;
using Glimpse.Core.Extensibility;
using Glimpse.Core.Message;
using Glimpse.Mvc.AlternateImplementation;
using Glimpse.Test.Common;
using Moq;
using Ploeh.AutoFixture;
using Xunit;
using Xunit.Extensions;

namespace Glimpse.Test.Mvc3.AlternateImplementation
{
    public class ViewEngineFindViewsShould
    {
        private readonly IFixture fixture = new Fixture();

        [Theory, AutoMock]
        public void ReturnAllMethodImplementationsWithAllMethods(ViewEngine sut)
        {
            var allMethods = sut.AllMethods();

            Assert.Equal(2, allMethods.Count());
        }

        [Fact]
        public void Construct()
        {
            var sut = new ViewEngine.FindViews(false);

            Assert.NotNull(sut);
            Assert.IsAssignableFrom<IAlternateImplementation<IViewEngine>>(sut);
        }

        [Fact]
        public void MethodToImplementIsRight()
        {
            var sut1 = new ViewEngine.FindViews(false);
            Assert.Equal("FindView", sut1.MethodToImplement.Name);

            var sut2 = new ViewEngine.FindViews(true);
            Assert.Equal("FindPartialView", sut2.MethodToImplement.Name);
        }

        [Theory, AutoMock]
        public void ProceedIfRuntimePolicyIsOff(ViewEngine.FindViews sut, IAlternateImplementationContext context)
        {
            context.Setup(c => c.RuntimePolicyStrategy).Returns(() => RuntimePolicy.Off);

            sut.NewImplementation(context);

            context.Verify(c => c.Proceed());
        }

        /* TODO nikmd23 not sure what you want to do here
        [Theory, AutoMock]
        public void PublishMessagesIfRuntimePolicyIsOnAndViewNotFound(ViewEngine.FindViews sut, IAlternateImplementationContext context)
        {
            context.Setup(c => c.Arguments).Returns(GetArguments());
            context.Setup(c => c.ReturnValue).Returns(new ViewEngineResult(Enumerable.Empty<string>()));

            sut.NewImplementation(context);

            context.MessageBroker.Verify(b => b.Publish(It.IsAny<ViewEngine.FindViews.Message>()));
            context.MessageBroker.Verify(b => b.Publish(It.IsAny<TimerResultMessage>()));
        }

        [Theory, AutoMock]
        public void PublishMessagesIfRuntimePolicyIsOnAndViewIsFound(ViewEngine.FindViews sut, IAlternateImplementationContext context, IView view, IViewEngine engine)
        {
            context.Setup(c => c.Arguments).Returns(GetArguments);
            context.Setup(c => c.ReturnValue).Returns(new ViewEngineResult(view, engine));
            context.ProxyFactory.Setup(p => p.IsProxyable(It.IsAny<object>())).Returns(true);
            context.ProxyFactory.Setup(p => 
                    p.CreateProxy(
                        It.IsAny<IView>(), 
                        It.IsAny<IEnumerable<IAlternateImplementation<IView>>>(), 
                        It.IsAny<object>()))
                    .Returns(view);

            sut.NewImplementation(context);

            context.ProxyFactory.Verify(p => p.IsProxyable(It.IsAny<object>()));
            context.Logger.Verify(l => l.Info(It.IsAny<string>(), It.IsAny<object[]>()));
            context.VerifySet(c => c.ReturnValue = It.IsAny<ViewEngineResult>());
            context.MessageBroker.Verify(b => b.Publish(It.IsAny<ViewEngine.FindViews.Message>()));
            context.MessageBroker.Verify(b => b.Publish(It.IsAny<TimerResultMessage>()));
        }
        */

        private object[] GetArguments()
        {
            return new object[]
                {
                    new ControllerContext(), 
                    fixture.CreateAnonymous("ViewName"), 
                    fixture.CreateAnonymous("MasterName"), 
                    fixture.CreateAnonymous<bool>()
                };
        }
    }
}