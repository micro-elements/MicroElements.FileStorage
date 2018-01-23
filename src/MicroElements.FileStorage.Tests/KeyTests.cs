using System;
using System.Linq.Expressions;
using System.Reflection;
using FluentAssertions;
using MicroElements.FileStorage.KeyAccessors;
using MicroElements.FileStorage.Tests.Models;
using Xunit;

namespace MicroElements.FileStorage.Tests
{
    public class KeyTests
    {
        [Fact]
        public void key_getter_should_get_id()
        {
            var person = new Person
            {
                Id = "persons/1",
                FirstName = "Bill",
                LastName = "Gates"
            };
            var idFunc = new DefaultKeyAccessor<Person>().GetIdFunc();
            var key = idFunc(person);
            key.Should().Be("persons/1");
        }

        [Fact]
        public void key_setter_should_set_id()
        {
            var person = new Person
            {
                Id = null,
                FirstName = "Bill",
                LastName = "Gates"
            };
            var idFunc = new DefaultKeyAccessor<Person>().SetIdFunc();
            idFunc(person, "persons/1");
            person.Id.Should().Be("persons/1");
        }

        private void SetAndRaise<TViewModel, TProperty>(
            TViewModel vm,
            Expression<Func<TViewModel, TProperty>> exp,
            TProperty value)
        {
            var propertyInfo = (PropertyInfo)((MemberExpression)exp.Body).Member;
            propertyInfo.SetValue(vm, value, null);
        }

        private Expression<Action> Assignment2<T>(Expression<Func<T>> lvalue, T rvalue)
        {
            var body = lvalue.Body;
            var c = Expression.Constant(rvalue, typeof(T));
            var a = Expression.Assign(body, c);
            return Expression.Lambda<Action>(a);
        }

        private Expression<Action> Assignment<TValue, TProperty>(
            Expression<Func<TValue, TProperty>> propExpression,
            TProperty rvalue)
        {
            var body = propExpression.Body;
            var c = Expression.Constant(rvalue, typeof(TProperty));
            var a = Expression.Assign(body, c);
            return Expression.Lambda<Action>(a);
        }

        [Fact]
        public void key_setter_for_int_property_should_set_id()
        {
            var person = new EntityWithIntId
            {
                Name = "Bill",
            };

            var keyAccessor = new KeyAccessor<EntityWithIntId>(p => p.Id.ToString(), (ent, id) => ent.Id = int.Parse(id));
            var personId = keyAccessor.GetIdFunc()(person);
            personId.Should().Be("0");
            person.Id.Should().Be(0);

            keyAccessor.SetIdFunc()(person, "2");
            person.Id.Should().Be(2);

            var defaultKeyAccessor = new DefaultKeyAccessor<EntityWithIntId>();
            person = new EntityWithIntId { Name = "Bill", };
            person.Id.Should().Be(0);
            defaultKeyAccessor.GetIdFunc()(person).Should().Be(null, "Id==0, but string key must be null");

            defaultKeyAccessor.SetIdFunc()(person, "1");
            person.Id.Should().Be(1);
        }
    }
}
