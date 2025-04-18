proto:
	@protoc --proto_path=shared/protos  \
			--csharp_out=shared/pb \
			shared/protos/**/*.proto


.PHONY: proto