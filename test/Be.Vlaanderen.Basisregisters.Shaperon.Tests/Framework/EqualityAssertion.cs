using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Threading;
using AutoFixture;
using AutoFixture.Idioms;
using AutoFixture.Kernel;

namespace Be.Vlaanderen.Basisregisters.Shaperon.Framework
{
    public class EqualityAssertion : IdiomaticAssertion
    {
        public EqualityAssertion(IFixture fixture)
        {
            Fixture = fixture ?? throw new ArgumentNullException(nameof(fixture));
            Comparer = Equals;
        }

        public IFixture Fixture { get; }
        public Func<object, object, bool> Comparer { get; }

        public override void Verify(ConstructorInfo constructorInfo)
        {
            if (constructorInfo == null)
            {
                throw new ArgumentNullException("constructorInfo");
            }

            var parameters = constructorInfo.GetParameters();
            var customizations = GetFreezingCustomizations(parameters);
            var constructorInfoSpecimenBuilder =
                new Postprocessor(
                    new ConstructorInfoSpecimenBuilder(),
                    new AutoPropertiesCommand()
                );
            using (new FixtureCustomizationsDisposable(Fixture))
            {
                Fixture.Customizations.Add(constructorInfoSpecimenBuilder);
                Fixture.Customize(
                    new CompositeCustomization(
                        GetFreezingCustomizations(constructorInfo.ReflectedType)));
                VerifyEqualReturnsTrue(constructorInfo, customizations);
                for (var i = 0; i < parameters.Length; ++i)
                {
                    VerifyEqualReturnsFalse(constructorInfo, parameters, customizations, i);
                }
            }
        }

        /// <summary>
        /// Verifies that the equality comparer returns the correct value when comparing
        /// 2 objects where the given property has a different value
        /// </summary>
        /// <param name="propertyInfo">The PropertyInfo to verify</param>
        public override void Verify(PropertyInfo propertyInfo)
        {
            if (propertyInfo == null)
            {
                throw new ArgumentNullException("propertyInfo");
            }

            if (!propertyInfo.CanWrite)
            {
                return;
            }

            var propertyInfos = GetWriteablePropertyInfo(propertyInfo.ReflectedType)
                .Where(pi => pi != propertyInfo);
            var fieldInfos = GetWriteableFieldsInfo(propertyInfo.ReflectedType);

            VerifyMember(propertyInfo, propertyInfos, fieldInfos, propertyInfo.ReflectedType, "property");
        }

        /// <summary>
        /// Verifies that the equality comparer returns the correct value when comparing
        /// 2 objects where the given field has a different value
        /// </summary>
        /// <param name="fieldInfo">The FieldInfo to verify</param>
        public override void Verify(FieldInfo fieldInfo)
        {
            if (fieldInfo == null)
            {
                throw new ArgumentNullException("fieldInfo");
            }

            if (fieldInfo.IsInitOnly || fieldInfo.IsLiteral)
            {
                return;
            }

            var propertyInfos = GetWriteablePropertyInfo(fieldInfo.ReflectedType);
            var fieldInfos = GetWriteableFieldsInfo(fieldInfo.ReflectedType)
                .Where(fi => fi != fieldInfo);

            VerifyMember(fieldInfo, propertyInfos, fieldInfos, fieldInfo.ReflectedType, "field");
        }

        private static PropertyInfo[] GetWriteablePropertyInfo(Type type)
        {
            return type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(pi => pi.CanWrite)
                .ToArray();
        }

        private static FieldInfo[] GetWriteableFieldsInfo(Type type)
        {
            return type.GetFields(BindingFlags.Public | BindingFlags.Instance)
                .Where(fi => !fi.IsInitOnly && !fi.IsLiteral)
                .ToArray();
        }

        private void VerifyEqualReturnsFalse(ConstructorInfo constructorInfo, ParameterInfo[] parameters,
            IEnumerable<ICustomization> customizations, int i)
        {
            using (new FixtureCustomizationsDisposable(Fixture))
            {
                Fixture.Customize(new CompositeCustomization(
                    new IndexedReplacement<ICustomization>(i, customizations).Expand(new EmptyCustomization())));
                var actual = CompareSpecimens(constructorInfo);
                if (actual)
                {
                    throw new EqualsOverrideException(
                        string.Format(CultureInfo.CurrentCulture,
                            "Equality is not properly implemented. Creating 2 instances of '{0}' with " +
                            "the constructor '{1}', with the same values except '{2}' returned true " +
                            "when applying the equality comparer, but false was expected.",
                            constructorInfo.ReflectedType.Name,
                            string.Join(",", parameters.Select(p => p.ParameterType.Name + " " + p.Name)),
                            parameters[i].Name));
                }
            }
        }

        private void VerifyEqualReturnsTrue(ConstructorInfo constructorInfo, IEnumerable<ICustomization> customizations)
        {
            using (new FixtureCustomizationsDisposable(Fixture))
            {
                Fixture.Customize(new CompositeCustomization(customizations));
                var actual = CompareSpecimens(constructorInfo);
                if (!actual)
                {
                    throw new EqualsOverrideException(
                        string.Format(CultureInfo.CurrentCulture,
                            "Equality is not properly implemented. Two same instances of {0} were " +
                            "compared but the equality method returned false.",
                            constructorInfo.ReflectedType));
                }
            }
        }

        private void VerifyMember(
            MemberInfo memberInfo,
            IEnumerable<PropertyInfo> propertyInfos,
            IEnumerable<FieldInfo> fieldInfos,
            Type type,
            string memberType)
        {
            var constructor = type.GetConstructors(BindingFlags.Public | BindingFlags.Instance).First();
            var parameters = constructor.GetParameters();
            var customizations = GetFreezingCustomizations(parameters)
                .Concat(GetPropertyInfoFreezingCustomizations(propertyInfos))
                .Concat(GetFieldInfoFreezingCustomizations(fieldInfos));
            var constructorInfoSpecimenBuilder =
                new Postprocessor(
                    new ConstructorInfoSpecimenBuilder(),
                    new AutoPropertiesCommand()
                );
            using (new FixtureCustomizationsDisposable(Fixture))
            {
                Fixture.Customizations.Add(constructorInfoSpecimenBuilder);
                Fixture.Customize(new CompositeCustomization(customizations));
                var actual = CompareSpecimens(constructor);
                if (actual)
                {
                    throw new EqualsOverrideException(
                        string.Format(CultureInfo.CurrentCulture,
                            "Equality is not properly implemented. Creating 2 instances of '{0}' " +
                            "with 2 different values in the {1} '{2}' returned true when " +
                            "applying the equality comparer, but false was expected.",
                            type.Name,
                            memberType,
                            memberInfo.Name));
                }
            }
        }

        private bool CompareSpecimens(object request)
        {
            var specimen = new Tuple<object, object>(
                Fixture.Create(request, new SpecimenContext(Fixture)),
                Fixture.Create(request, new SpecimenContext(Fixture))
            );
            return Comparer(specimen.Item1, specimen.Item2);
        }

        private ICustomization[] GetFreezingCustomizations(IEnumerable<ParameterInfo> parameters)
        {
            return parameters.Select(p =>
                    (ICustomization) new FreezeOnMatchCustomization(
                        p.ParameterType,
                        new EqualsRequestSpecification(p)
                    ))
                .ToArray();
        }

        private ICustomization[] GetFreezingCustomizations(Type type)
        {
            var bindingFlag = BindingFlags.Public | BindingFlags.Instance;
            return GetPropertyInfoFreezingCustomizations(type.GetProperties(bindingFlag))
                .Concat(GetFieldInfoFreezingCustomizations(type.GetFields(bindingFlag)))
                .ToArray();
        }

        private ICustomization[] GetPropertyInfoFreezingCustomizations(IEnumerable<PropertyInfo> propertyInfos)
        {
            return propertyInfos.Select(pi => (ICustomization) new FreezeOnMatchCustomization(
                    pi.PropertyType,
                    new EqualsRequestSpecification(pi))
                )
                .ToArray();
        }

        private ICustomization[] GetFieldInfoFreezingCustomizations(IEnumerable<FieldInfo> fieldInfos)
        {
            return fieldInfos.Select(fi => (ICustomization) new FreezeOnMatchCustomization(
                    fi.FieldType,
                    new EqualsRequestSpecification(fi))
                )
                .ToArray();
        }

        /// <summary>
        /// Specification which is statisfied when the request equal the request given in the constructor
        /// </summary>
        private class EqualsRequestSpecification : IRequestSpecification
        {
            private object _frozenRequest;

            /// <summary>
            /// Creates a new instance of EqualsRequestSpecification
            /// </summary>
            /// <param name="frozenRequest">The request to match</param>
            public EqualsRequestSpecification(object frozenRequest)
            {
                if (frozenRequest == null)
                {
                    throw new ArgumentNullException("frozenRequest");
                }

                this._frozenRequest = frozenRequest;
            }

            public bool IsSatisfiedBy(object request)
            {
                return request.Equals(_frozenRequest);
            }
        }

        /// <summary>
        /// When disposed, remove all customizations that were added to the fixture
        /// after this class is instanciated
        /// </summary>
        private class FixtureCustomizationsDisposable : IDisposable
        {
            private readonly IFixture _fixture;
            private List<ISpecimenBuilder> _customizations;

            public FixtureCustomizationsDisposable(IFixture fixture)
            {
                if (fixture == null) throw new ArgumentNullException("fixture");
                this._fixture = fixture;
                _customizations = fixture.Customizations.ToList();
            }

            public void Dispose()
            {
                if (this._customizations == null)
                {
                    return;
                }

                var customizations = Interlocked.Exchange(ref this._customizations, null);
                foreach (var customization in _fixture.Customizations.Except(customizations))
                {
                    _fixture.Customizations.Remove(customization);
                }
            }
        }

        /// <summary>
        /// An empty customization which does nothing
        /// </summary>
        private class EmptyCustomization : ICustomization
        {
            public void Customize(IFixture fixture)
            {
            }
        }

        /// <summary>
        /// Creates a value based on a <see cref="System.Reflection.ConstructorInfo"/>ConstructorInfo</cref>.
        /// </summary>
        private class ConstructorInfoSpecimenBuilder : ISpecimenBuilder
        {
            /// <summary>
            /// Creates a value when the given request is an instance of <see cref="ConstructorInfo" />
            /// </summary>
            /// <param name="request">The request object.</param>
            /// <param name="context">The context used to resolve requests.</param>
            /// <returns>An instance of the type which the ConstructorInfo belongs to.</returns>
            public object Create(object request, ISpecimenContext context)
            {
                var constructorInfo = request as ConstructorInfo;
                if (constructorInfo == null)
                {
                    return new NoSpecimen();
                }

                var args = constructorInfo.GetParameters()
                    .Select(p => context.Resolve(p))
                    .ToArray();
                return constructorInfo.Invoke(args);
            }
        }
    }
}
