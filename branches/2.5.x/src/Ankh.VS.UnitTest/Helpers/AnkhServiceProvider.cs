// $Id$
//
// Copyright 2009 The AnkhSVN Project
//
//  Licensed under the Apache License, Version 2.0 (the "License");
//  you may not use this file except in compliance with the License.
//  You may obtain a copy of the License at
//
//    http://www.apache.org/licenses/LICENSE-2.0
//
//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an "AS IS" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//  limitations under the License.

using System;
using System.Collections.Generic;
using System.Text;
using Ankh;
using System.ComponentModel.Design;

namespace AnkhSvn_UnitTestProject.Helpers
{
    class AnkhServiceProvider : IAnkhServiceProvider
    {
        internal ServiceContainer sc = new ServiceContainer();

        public void AddService(Type serviceType, object serviceInstance)
        {
            sc.AddService(serviceType, serviceInstance);
        }

        public T GetService<T>() where T : class
        {
            return (T)sc.GetService(typeof(T));
        }

        public T GetService<T>(Type serviceType) where T : class
        {
            return (T)sc.GetService(serviceType);
        }

        public object GetService(Type serviceType)
        {
            return sc.GetService(serviceType);
        }

    }
}
