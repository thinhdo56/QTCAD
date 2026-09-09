using Inventor;
using QTCAD.API.Commands;
using QTCAD.Inv25.Adapter;
using QTCAD.Inv25.Services;
using System;
using System.Runtime.Versioning;
namespace QTCAD.Inv25.UI
{
    public sealed class RibbonManager
    {
        private readonly InventorContext _context;
        private readonly CommandRegistry _commands;
        private ButtonDefinition? _documentValidationButton;
        private ButtonDefinition? _testButton;
        private ButtonDefinition? _propertyButton;
        private ButtonDefinition? _geometryAnalysisButton;
        public RibbonManager(InventorContext context)
        {
            _context = context;
            _commands = new CommandRegistry(context);
        }

        public void Initialize()
        {
            CommandManager commandManager = _context.Application.CommandManager;

            ICommand testCommand = _commands.Get("QTCAD.Test");
            ICommand documentValidationCommand = _commands.Get("QTCAD.DocumentValidation");
            ICommand propertyCommand = _commands.Get("QTCAD.Properties");
            ICommand geometryAnalysisCommand = _commands.Get("QTCAD.GeometryAnalysis");

            _testButton = GetOrCreateButtonDefinition(commandManager, testCommand);
            _documentValidationButton = GetOrCreateButtonDefinition(commandManager, documentValidationCommand);
            _propertyButton = GetOrCreateButtonDefinition( commandManager, propertyCommand);
            _geometryAnalysisButton = GetOrCreateButtonDefinition(commandManager, geometryAnalysisCommand);

            _testButton.OnExecute += TestButton_OnExecute;
            _documentValidationButton.OnExecute += DocumentValidationButton_OnExecute;
            _propertyButton.OnExecute += PropertyButton_OnExecute;
            _geometryAnalysisButton.OnExecute += GeometryAnalysisButton_OnExecute;  

            CreateRibbon(RibbonEnvironment.Part);
            CreateRibbon(RibbonEnvironment.Assembly);
            CreateRibbon(RibbonEnvironment.Drawing);
        }
        private void CreateRibbon(RibbonEnvironment environment)
        {
            UserInterfaceManager uiManager = _context.Application.UserInterfaceManager;
            Ribbon ribbon = uiManager.Ribbons[environment.GetRibbonName()];

            RibbonTab tab;

            try
            {
                tab = ribbon.RibbonTabs["QTCAD.Tab"];
            }
            catch
            {
                tab = ribbon.RibbonTabs.Add(
                    "QTCAD",
                    "QTCAD.Tab",
                    Guid.NewGuid().ToString());
            }

            RibbonPanel panel;

            try
            {
                panel = tab.RibbonPanels["QTCAD.Panel"];
            }
            catch
            {
                panel = tab.RibbonPanels.Add(
                    "Tools",
                    "QTCAD.Panel",
                    Guid.NewGuid().ToString());
            }

            if (!CommandControlExists(panel, _testButton))
            {
                panel.CommandControls.AddButton(_testButton, true);
            }

            if (!CommandControlExists(panel, _documentValidationButton))
            {
                panel.CommandControls.AddButton(_documentValidationButton, true);
            }

            if (!CommandControlExists(panel, _propertyButton))
            {
                panel.CommandControls.AddButton(_propertyButton, true);
            }
            if (!CommandControlExists(panel, _geometryAnalysisButton))
            {
                panel.CommandControls.AddButton(_geometryAnalysisButton, true);
            }
        }

        private bool CommandControlExists(RibbonPanel panel, ButtonDefinition? button)
        {
            if (button == null)
            {
                return false;
            }

            for (int i = 1; i <= panel.CommandControls.Count; i++)
            {
                CommandControl control = panel.CommandControls[i];

                if (control.ControlDefinition.InternalName == button.InternalName)
                {
                    return true;
                }
            }

            return false;
        }
        private ButtonDefinition GetOrCreateButtonDefinition(CommandManager commandManager,ICommand command)
        {
            try
            {
                return commandManager.ControlDefinitions[command.Id] as ButtonDefinition
                    ?? throw new InvalidOperationException(
                        $"ControlDefinition '{command.Id}' is not a ButtonDefinition.");
            }
            catch
            {
                return commandManager.ControlDefinitions.AddButtonDefinition(
                    command.DisplayName,
                    command.Id,
                    CommandTypesEnum.kNonShapeEditCmdType,
                    Guid.NewGuid().ToString(),
                    command.Description,
                    command.DisplayName);
            }
        }
        private void TestButton_OnExecute(NameValueMap context)
        {
            ICommand command = _commands.Get("QTCAD.Test");
            command.Execute();
        }

        private void DocumentValidationButton_OnExecute(NameValueMap context)
        {
            ICommand command = _commands.Get("QTCAD.DocumentValidation");
            command.Execute();
        }

        private void PropertyButton_OnExecute(NameValueMap context)
        {
            _commands.Get("QTCAD.Properties").Execute();
        }

        private void GeometryAnalysisButton_OnExecute(NameValueMap context)
        {
            _commands.Get("QTCAD.GeometryAnalysis").Execute();
        }
        public void Dispose()
        {
            if (_testButton != null)
            {
                _testButton.OnExecute -= TestButton_OnExecute;
            }
            if (_documentValidationButton != null)
            {
                _documentValidationButton.OnExecute -= DocumentValidationButton_OnExecute;
            }
            if (_propertyButton != null)
            {
                _propertyButton.OnExecute -= PropertyButton_OnExecute;
            }
            if (_geometryAnalysisButton != null)
            {
                _geometryAnalysisButton.OnExecute -= GeometryAnalysisButton_OnExecute;
            }
        }
    }
}
