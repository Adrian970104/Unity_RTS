public interface ISelectable
{
    // Kijelölés kezdete
    void OnSelectStart();

    // Kijelölés vége
    void OnSelectEnd();

    // Elsődleges action (pl. bal gombra)
    void OnAction();
}
