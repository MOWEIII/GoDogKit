using System.Collections.Generic;

namespace GoDogKit.Architecture;

public interface IMVCModel;

public interface IMVCView<TModel> where TModel : IMVCModel
{
    public void Update(TModel model);
}

public class MVCController<TModel, TView> where TModel : IMVCModel where TView : IMVCView<TModel>
{
    public Dictionary<TView, TModel> Bindings { get; protected set; }

    public void Bind(TModel model, TView view) => Bindings.Add(view, model);

    public bool Unbind(TView view) => Bindings.Remove(view);

    public void Update(TView view) => view.Update(Bindings[view]);

    public void UpdateAll()
    {
        foreach (var view in Bindings.Keys)
        {
            view.Update(Bindings[view]);
        }
    }
}