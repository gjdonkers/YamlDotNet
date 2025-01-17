﻿// This file is part of YamlDotNet - A .NET library for YAML.
// Copyright (c) Antoine Aubry and contributors
//
// Permission is hereby granted, free of charge, to any person obtaining a copy of
// this software and associated documentation files (the "Software"), to deal in
// the Software without restriction, including without limitation the rights to
// use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies
// of the Software, and to permit persons to whom the Software is furnished to do
// so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

using System.Collections.Generic;
using FluentAssertions;
using Xunit;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace YamlDotNet.Test.Serialization
{
    public class DeserializerTest
    {
        [Fact]
        public void Deserialize_YamlWithInterfaceTypeAndMapping_ReturnsModel()
        {
            var yaml = @"
name: Jack
cars:
- name: Mercedes
  year: 2018
- name: Honda
  year: 2021
";

            var sut = new DeserializerBuilder()
                .WithNamingConvention(CamelCaseNamingConvention.Instance)
                .WithTypeMapping<ICar, Car>()
                .Build();

            var person = sut.Deserialize<Person>(yaml);
            person.Name.Should().Be("Jack");
            person.Cars.Should().HaveCount(2);
            person.Cars[0].Name.Should().Be("Mercedes");
            person.Cars[0].Spec.Should().BeNull();
            person.Cars[1].Name.Should().Be("Honda");
            person.Cars[1].Spec.Should().BeNull();
        }

        [Fact]
        public void Deserialize_YamlWithTwoInterfaceTypesAndMappings_ReturnsModel()
        {
            var yaml = @"
name: Jack
cars:
- name: Mercedes
  year: 2018
  spec:
    engineType: V6
    driveType: AWD
- name: Honda
  year: 2021
  spec:
    engineType: V4
    driveType: FWD
";

            var sut = new DeserializerBuilder()
                .WithNamingConvention(CamelCaseNamingConvention.Instance)
                .WithTypeMapping<ICar, Car>()
                .WithTypeMapping<IModelSpec, ModelSpec>()
                .Build();

            var person = sut.Deserialize<Person>(yaml);
            person.Name.Should().Be("Jack");
            person.Cars.Should().HaveCount(2);
            person.Cars[0].Name.Should().Be("Mercedes");
            person.Cars[0].Spec.EngineType.Should().Be("V6");
            person.Cars[0].Spec.DriveType.Should().Be("AWD");
            person.Cars[1].Name.Should().Be("Honda");
            person.Cars[1].Spec.EngineType.Should().Be("V4");
            person.Cars[1].Spec.DriveType.Should().Be("FWD");
        }

        public class Person
        {
            public string Name { get; private set; }

            public IList<ICar> Cars { get; private set; }
        }

        public class Car : ICar
        {
            public string Name { get; private set; }

            public int Year { get; private set; }

            public IModelSpec Spec { get; private set; }
        }

        public interface ICar
        {
            string Name { get; }

            int Year { get; }
            IModelSpec Spec { get; }
        }

        public class ModelSpec : IModelSpec
        {
            public string EngineType { get; private set; }

            public string DriveType { get; private set; }
        }

        public interface IModelSpec
        {
            string EngineType { get; }

            string DriveType { get; }
        }
    }
}
