var StringComponent = React.createClass({
	displayName: "StringComponent",
	getInitialState: function() 
	{
		//return { email: this.props.initialEmail, password: this.props.initialPassword };
		return { };
	},
	componentWillMount:function()
	{
		
	},
	onChange : function(e)
	{
		this.setState({ bound_data: e.target.value });
	},
	render: function() 
	{
		return React.createElement(
			"p",{ key: this.props.metadata.name},
			this.props.metadata.prompt,
			' ',
			React.createElement("input", 
			{ 
				onChange:this.onChange, 
				type:"text", 
				ref : this.props.metadata.name,
				"defaultValue": this.props.metadata.name
			})
		);
	}
});