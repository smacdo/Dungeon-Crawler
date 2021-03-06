﻿/*
 * Copyright 2012-2017 Scott MacDonald
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 * http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Forge.Settings
{
    /// <summary>
    ///  Runtime game settings.
    ///  TODO: This doens't need to exist in custom namespace.
    /// </summary>
    public class ForgeSettings
    {
        public bool DrawCollisionDebug { get; set; } = false;
        public bool DrawPhysicsDebug { get; set; } = false;
        public bool DrawSpriteDebug { get; set; } = false;
        public bool DrawTransformDebug { get; set; } = false;
        public bool DrawWeaponHitDebug { get; set; } = true;
    }
}
