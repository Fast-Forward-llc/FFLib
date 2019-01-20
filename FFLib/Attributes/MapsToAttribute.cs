/*******************************************************
 * Project: FFLib V1.0
 * Title: MapsToAttribute.cs
 * Author: Phillip Bird of Fast Forward,LLC
 * Copyright © 2012 Fast Forward, LLC. 
 * Dual licensed under the MIT or GPL Version 2 licenses.
 * Use of any component of FFLib requires acceptance and adhearance 
 * to the terms of either the MIT License or the GNU General Public License (GPL) Version 2 exclusively.
 * Notification of license selection is not required and will be infered based on applicability.
 * Contributions to FFLib requires a contributor grant on file with Fast Forward, LLC.
********************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FFLib.Attributes
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field )]
    public class MapsToAttribute : Attribute
    {

        public MapsToAttribute(string PropertyName)
        {
            this._propertyName = PropertyName;
        }

        private string _propertyName;

        public string PropertyName
        {
            get { return _propertyName; }
            set { _propertyName = value; }
        }

    }
}
