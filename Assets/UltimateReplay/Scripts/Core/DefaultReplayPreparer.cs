﻿using System;
using System.Collections.Generic;
using System.Reflection;
using UltimateReplay.Core.StatePreparer;
using UnityEngine;

namespace UltimateReplay.Core
{
    /// <summary>
    /// The default <see cref="IReplayPreparer"/> used by Ultimate Replay to prepare game objects for gameplay and playback.
    /// </summary>
    public class DefaultReplayPreparer : IReplayPreparer
    {
        // Private
        private HashSet<ComponentPreparer> preparers = new HashSet<ComponentPreparer>();

        // Private
        private static readonly Type[] skipTypes =
        {
            typeof(ReplayObject),
            typeof(ReplayBehaviour),
            typeof(Camera),
            typeof(AudioSource),
            typeof(ParticleSystem),
        };

        // Constructor
        public DefaultReplayPreparer()
        {
            // Load all preparers
#if UNITY_WINRT && !UNITY_EDITOR
            foreach (Type type in typeof(ReplayManager).GetTypeInfo().Assembly.GetTypes())
#else
            foreach (Type type in typeof(ReplayManager).Assembly.GetTypes())
#endif
            {
                // Check for attribute
#if UNITY_WINRT && !UNITY_EDITOR
                if(type.GetTypeInfo().GetCustomAttribute<ReplayComponentPreparerAttribute>() != null)
#else
                if (type.IsDefined(typeof(ReplayComponentPreparerAttribute), false) == true)
#endif
                {
#if UNITY_WINRT && !UNITY_EDITOR
                    ReplayComponentPreparerAttribute attribute = type.GetTypeInfo().GetCustomAttribute<ReplayComponentPreparerAttribute>();
#else
                    // Get the attributes
                    object[] data = type.GetCustomAttributes(typeof(ReplayComponentPreparerAttribute), false);

                    // Get the attribute
                    ReplayComponentPreparerAttribute attribute = data[0] as ReplayComponentPreparerAttribute;
#endif

                    // Make sure the type inheirts from ComponentPreparer
                    if(typeof(ComponentPreparer).IsAssignableFrom(type) == false)
                    {
                        Debug.LogWarning(string.Format("Custom replay component preparer '{0}' must inherit from ComponentPreparer<>", type));
                        continue;
                    }

                    // Create an instance
                    ComponentPreparer preparer = null;

                    try
                    {
                        // Try to create an instance
                        preparer = (ComponentPreparer)Activator.CreateInstance(type);
                    }
                    catch
                    {
                        Debug.LogWarning(string.Format("Failed to create an instance of custom replay component preparer '{0}'. Make sure the type has a default constructor", type));
                        continue;
                    }
                    
                    // Cache the attribute
                    preparer.Attribute = attribute;

                    // Register the preparer
                    preparers.Add(preparer);
                }
            }
        }

        // Methods
        /// <summary>
        /// Prepare the specified replay object for playback mode.
        /// </summary>
        /// <param name="replayObject">The replay object to prepare</param>
        public virtual void PrepareForPlayback(ReplayObject replayObject)
        {
            // Find all components on the object
            foreach (Component component in replayObject.GetComponentsInChildren<Component>())
            {
                // Make sure the component is still alive
                if (component == null)
                    return;

                bool skip = false;

                // Check if the component should be prepared or skipped
                foreach (Type skipType in skipTypes)
                {
                    // Check if the component is a skip type or child of
                    if (skipType.IsInstanceOfType(component))
                    {
                        // Set the skip flag
                        skip = true;
                        break;
                    }
                }

                // Check if we should skip the component
                if (skip == true)
                    continue;

                // Get the component type
                Type componentType = component.GetType();

                // Try to find a preparer
                ComponentPreparer preparer = FindPreparer(componentType);

                // Check for error
                if (preparer == null)
                    continue;

                // Prepare the component
                preparer.InvokePrepareForPlayback(component);
            }
        }

        /// <summary>
        /// Prepare the specified replay object for gameplay mode.
        /// </summary>
        /// <param name="replayObject">The replay object to prepare</param>
        public virtual void PrepareForGameplay(ReplayObject replayObject)
        {
            // Find all components on the object
            foreach (Component component in replayObject.GetComponentsInChildren<Component>())
            {
                // Make sure the component is still alive
                if (component == null)
                    return;

                bool skip = false;

                // Check if the component should be prepared or skipped
                foreach(Type skipType in skipTypes)
                {
                    // Check if the component is a skip type or child of
                    if (skipType.IsInstanceOfType(component))
                    {
                        // Set the skip flag
                        skip = true;
                        break;
                    }
                }

                // Check if we should skip the component
                if (skip == true)
                    continue;

                // Get the component type
                Type componentType = component.GetType();

                // Try to find a preparer
                ComponentPreparer preparer = FindPreparer(componentType);

                // Check for error
                if (preparer == null)
                    continue;

                // Prepare the component
                preparer.InvokePrepareForGameplay(component);
            }
        }

        private ComponentPreparer FindPreparer(Type componentType)
        {
            foreach(ComponentPreparer preparer in preparers)
            {
                if(preparer.Attribute.componentType.IsAssignableFrom(componentType) == true)
                {
                    return preparer;
                }
            }

            // No preparer found
            return null;
        }
    }
}
