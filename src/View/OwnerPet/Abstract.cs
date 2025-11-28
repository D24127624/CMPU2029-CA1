
namespace kms.View.OwnerPet
{

    using Terminal.Gui;
    using static Terminal.Gui.TableView;

    using kms.Util;
    using kms.Models;
    using kms.Services;

    /// <summary>
    /// Abstract owner/pet window class with shared UI code
    /// </summary>
    public abstract class AbstractOwnerPetWindow : AbstractWindow
    {

        protected readonly IOwnerService _ownerService;
        protected readonly IPetService _petService;

        public AbstractOwnerPetWindow(
            BaseNavigation navigation,
            string title,
            IOwnerService ownerService,
            IPetService petService
        ) : base(navigation, title)
        {
            _ownerService = ownerService;
            _petService = petService;
        }

        protected TableView? ShowOwnersTable(FrameView parent, List<Owner> results, Action<FrameView, Button?, int, Owner>? selectionAction = null)
        {
            if (!NoResultsFound<Owner>(parent, results, "No owners found for the provided query!"))
            {
                List<Dictionary<string, object?>> values = [];
                foreach (Owner owner in results)
                {
                    values.Add(new()
                    {
                        {"ID", owner.Id},
                        {"Name", owner.Name},
                        {"Phone", owner.PhoneNumber},
                        {"Address", owner.Address}
                    });
                }

                TableView ownerTable = ViewUtils.CreateTable(parent, 1, 1, Dim.Fill() - 2, Dim.Fill() - 1, values);
                Button? action = null;
                ownerTable.SelectedCellChanged += (args) =>
                {
                    int id = (int)args.Table.Rows[ownerTable.SelectedRow][0];
                    selectionAction?.Invoke(parent, action, id, results.First(o => o.Id == id));
                };
                return ownerTable;
            }
            return null;
        }

        protected TableView? ShowPetsTable(FrameView parent, List<Pet> results, Action<FrameView, Button?, int, Pet>? selectionAction = null, bool simple = true)
        {
            if (!NoResultsFound<Pet>(parent, results, "No pets found for the provided query!"))
            {
                List<Dictionary<string, object?>> values = [];
                foreach (Pet pet in results)
                {
                    Dictionary<string, object?> entry = [];
                    entry["ID"] = pet.Id;
                    entry["Name"] = pet.Name;
                    entry["Age"] = pet.Age;
                    entry["Type"] = $"{pet.PetType}";
                    entry["Size"] = pet.PetType == PetType.CAT ? $"{Models.Size.SMALL}" : $"{((Dog)pet).Size}";
                    if (!simple)
                    {
                        entry["Breed"] = pet.Breed;
                        entry["Food Preferences"] = pet.FoodPreferences;
                        entry["Walking Preferences"] = pet.PetType == PetType.CAT ? "" : $"{((Dog)pet).WalkingPreference}";
                    }
                    values.Add(entry);
                }
                TableView petTable = ViewUtils.CreateTable(parent, 1, 1, Dim.Fill() - 2, Dim.Fill() - 1, values);
                Button? action = null;
                petTable.SelectedCellChanged += (args) =>
                {
                    int id = (int)args.Table.Rows[petTable.SelectedRow][0];
                    selectionAction?.Invoke(parent, action, id, results.First(o => o.Id == id));
                };
                return petTable;
            }
            return null;
        }

    }

}
