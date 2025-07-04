// using Scribbly.Eventually.UnitTests.Setup.Aggregates;
//
// namespace BeerSender.Domain.Boxes.Handlers;
//
// class Apply_shipping_label_handler : CommandHandler<Apply_shipping_label, Box>
// {
//     public Apply_shipping_label_handler(
//         IEnumerable<Event> event_stream,
//         Action<Event> publish_event)
//         : base(event_stream, publish_event)
//     { }
//
//     protected override IEnumerable<Event> Handle_command(
//         Box aggregate,
//         Apply_shipping_label command)
//     {
//         if (command.Label.Is_valid())
//             yield return new Label_applied(command.Label);
//         else
//             yield return new Label_was_invalid();
//     }
// }