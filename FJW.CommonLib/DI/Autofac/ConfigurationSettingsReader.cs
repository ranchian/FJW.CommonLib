//using System;
//using System.Configuration;
//using System.Globalization;
//using Autofac;
//using Autofac.Configuration;

//namespace FJW.DI
//{
//    public class ConfigurationSettingsReader : Module
//    {
//        public IConfigurationRegistrar ConfigurationRegistrar { get; set; }

//        /// <summary>Gets the section handler.</summary>
//        /// <value>
//        /// The <see cref="T:Autofac.Configuration.SectionHandler" /> that will be converted into
//        /// component registrations in a container.
//        /// </value>
//        public SectionHandler SectionHandler { get; protected set; }
//        /// <summary>
//        /// Initializes a new instance of the <see cref="T:Autofac.Configuration.ConfigurationSettingsReader" /> class
//        /// using the default application configuration file with a configuration section named <c>autofac</c>.
//        /// </summary>
//        public ConfigurationSettingsReader()
//            : this("autofac")
//        {
//        }

//        /// <summary>
//        /// Initializes a new instance of the <see cref="T:Autofac.Configuration.ConfigurationSettingsReader" /> class
//        /// using the default application configuration file and a named section.
//        /// </summary>
//        /// <param name="sectionName">
//        /// The name of the configuration section corresponding to a <see cref="T:Autofac.Configuration.SectionHandler" />.
//        /// </param>
//        public ConfigurationSettingsReader(string sectionName)
//        {
//            if (sectionName == null)
//                throw new ArgumentNullException("sectionName");
//            this.SectionHandler = (SectionHandler)ConfigurationManager.GetSection(sectionName);
//            if (this.SectionHandler == null)
//                throw new ConfigurationErrorsException(string.Format((IFormatProvider)CultureInfo.CurrentCulture, "SectionNotFound", new object[1]
//        {
//          (object) sectionName
//        }));
//        }

//        /// <summary>
//        /// Initializes a new instance of the <see cref="T:Autofac.Configuration.ConfigurationSettingsReader" /> class
//        /// using a named configuration file and section.
//        /// </summary>
//        /// <param name="sectionName">
//        /// The name of the configuration section corresponding to a <see cref="T:Autofac.Configuration.SectionHandler" />.
//        /// </param>
//        /// <param name="configurationFile">
//        /// The <c>app.config</c>/<c>web.config</c> format configuration file containing the
//        /// named section.
//        /// </param>
//        public ConfigurationSettingsReader(string sectionName, string configurationFile)
//        {
//            this.SectionHandler = SectionHandler.Deserialize(configurationFile, sectionName);
//        }
//        /// <summary>
//        /// 
//        /// </summary>
//        /// <param name="builder"></param>
//        protected override void Load(ContainerBuilder builder)
//        {
//            if (builder == null)
//                throw new ArgumentNullException("builder");
//            if (this.SectionHandler == null)
//                throw new InvalidOperationException("InitializeSectionHandler");
//            (this.ConfigurationRegistrar ?? new ConfigurationRegistrar()).RegisterConfigurationSection(builder, this.SectionHandler);
//        }
//    }
//}
