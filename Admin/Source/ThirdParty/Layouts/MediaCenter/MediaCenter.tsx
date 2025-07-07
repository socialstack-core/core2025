import Loop from 'UI/Loop';
import Container from 'UI/Container';
import Row from 'UI/Row';
import Column from 'UI/Column';
import Input from 'UI/Input';
import Image from 'UI/Image';
import Loading from 'UI/Loading';
import Search from 'UI/Search';
import SubHeader from 'Admin/SubHeader';
import Uploader from 'UI/Uploader';
import ConfirmModal from 'UI/Modal/ConfirmModal';
import Modal from 'UI/Modal';
import * as fileRef from 'UI/FileRef';
import MultiSelect from 'Admin/MultiSelect';
import uploadApi, { Upload } from 'Api/Upload';
import { useState, useEffect } from 'react';
import { Tag } from 'Api/Tag';

var fields = ['id', 'originalName'];
var searchFields = ['originalName', 'alt', 'author', 'id'];

const CLOSEST_MULTIPLE = 2;
const PREVIEW_SIZE = 512;
const UPLOAD_SINGLE = 1;
const UPLOAD_MULTIPLE = 2;

type UploadModal = {
    uploadMode: number,
    bulkUploaded: boolean,
    focalX: number,
    focalY: number,
    uploadId?: uint,
    alt?: string,
    author?: string,
    tags?: Tag[],
    transcodeState?: int,
    existingFileRef?: string,
    originalName?: string
};

const MediaCenter = (props) => {
    const [sorter, setSort] = useState({ field: 'id', direction: 'desc' });
    const [bulkSelections, setBulkSelections] = useState<Record<int, boolean> | null>(null);
    const [confirmDelete, setConfirmDelete] = useState<boolean>(false);
    const [uploadModal, setUploadModal] = useState<UploadModal | null>(null);
    const [deleting, setDeleting] = useState<boolean>(false);
    const [deleteCount, setDeleteCount] = useState<number>(0);
    const [deleteFailed, setDeleteFailed] = useState<boolean>(false);
    const [searchFilter, setSearchFilter] = useState<string | null>(null);
    const [filterTagId, setFilterTagId] = useState<uint | null>(null);
    const [uploads, setUploads] = useState<Upload[] | null>(null);

    let sort = sorter;

    if (sorter && !fields.find(field => field == sort.field)) {
        // Restore to id sort:
        sort = { field: 'id', direction: 'desc' };
    }

    useEffect(() => {
        var combinedFilter = {};

        if (!combinedFilter.sort && sort) {
            combinedFilter.sort = sort;
        }

        if (filterTagId) {
            combinedFilter.query = "Tags contains ?"
            combinedFilter.args = [];
            combinedFilter.args.push(filterTagId);
        }

        if (searchFilter && searchFilter.length > 0 && searchFields) {
            var searchQuery = '';
            var searchQueryArgs = [];
            var searchDelimiter = '';

            for (var i = 0; i < searchFields.length; i++) {

                var field = searchFields[i];
                var fieldNameUcFirst = field.charAt(0).toUpperCase() + field.slice(1);

                if (fieldNameUcFirst == "Id") {
                    if (/^\d+$/.test(searchFilter)) {

                        searchQuery = searchQuery + searchDelimiter + fieldNameUcFirst + " =?"
                        searchQueryArgs.push(searchFilter);
                        searchDelimiter = ' OR ';
                    }
                } else {
                    searchQuery = searchQuery + searchDelimiter + fieldNameUcFirst + " contains ?"
                    searchQueryArgs.push(searchFilter);

                    searchDelimiter = ' OR ';
                }
            }

            if (searchQuery.length > 0) {
                if (!combinedFilter.query) {
                    combinedFilter.query = searchQuery;
                    combinedFilter.args = searchQueryArgs;
                } else {
                    combinedFilter.query = combinedFilter.query + ' AND (' + searchQuery + ')';
                    searchQueryArgs.forEach(arg => combinedFilter.args.push(arg));
                }
            }
        }

        combinedFilter.pageSize = 60;

        uploadApi.list(combinedFilter, [uploadApi.includes.tags]).then(uploads => {
            setUploads(uploads.results);
            setBulkSelections(null);
        });

    }, [filterTagId, searchFilter, sorter, deleteCount]);

    const showRef = (ref: string, size: number = 256) => {
        var parsedRef = fileRef.parse(ref);

        if (!parsedRef) {
            return null;
        }

        var targetSize : number | undefined = size;
        //var minSize = size == 256 ? 238 : size;

        var fileClassName = parsedRef.fileType != '' ? 'fal fa-4x fa-file fa-file-' + parsedRef.fileType : 'fal fa-4x fa-file';

        // Check if it's an image/ video/ audio file. If yes, a preview is shown. Otherwise it'll be a placeholder icon.
        var canShowImage = parsedRef.isImage();

        if (canShowImage) {
			var argW = parsedRef.getNumericArg('w', 0);
			var argH = parsedRef.getNumericArg('h', 0);
			
            if ((argW && argW < size) && (argH && argH < size)) {
                targetSize = undefined;
            }

        }

        return canShowImage ?
            <Image fileRef={ref} size={targetSize} portraitCheck /> :
            <span className={fileClassName}></span>;

    }

    const renderTags = (uploads : Upload[] | null) => {
        if (!uploads) {
            return null;
        }

        var tags : Tag[] = [];

        uploads.map(media => {
            media.tags?.map(tag => {
                if (!tags.includes(tag)) {
                    tags.push(tag);
                }
            });
        });

        return (
            <ul className='media-center__tags'>
                {tags.map(renderTag)}
            </ul>
        )
    }

    const renderTag = (tag: Tag) => {
        if (!tag || !tag.name || tag.name.length == 0) {
            return ('');
        }

        var tagClassName = (filterTagId == tag.id) ? "media-center__tag media-center__tag-selected" : "media-center__tag"
        return (
            <li className={tagClassName} onClick={() => {
                if (filterTagId == tag.id) {
                    setFilterTagId(null);
                } else {
                    setFilterTagId(tag.id);
                }
            }}>
                {tag.name}
            </li>
        );
    }

    const renderHeader = (allContent) => {
        // Header (Optional)
        var heads = fields.map(field => {

            // Class for styling the sort buttons:
            var sortingByThis = sort && sort.field == field;
            var className = '';

            if (sortingByThis) {
                className = sort.direction == 'desc' ? 'sorted-desc' : 'sorted-asc';
            }

            // Field name with its first letter uppercased:
            var ucFirstFieldName = field.length ? field.charAt(0).toUpperCase() + field.slice(1) : '';

            return (
                <th className={className}>
                    {ucFirstFieldName} <i className="fa fa-caret-down" onClick={() => {
                        // Sort desc
                        setSort({
                            field,
                            direction: 'desc'
                        });
                    }} /> <i className="fa fa-caret-up" onClick={() => {
                        // Sort asc
                        setSort({
                            field,
                            direction: 'asc'
                        });
                    }} />
                </th>
            );
        });

        // If everything in allContent is selected, mark this as selected as well.
        var checked = false;

        if (bulkSelections && allContent.length) {
            checked = true;
            allContent.forEach(e => {
                if (!bulkSelections[e.id]) {
                    checked = false;
                }
            });
        }

        return [
            <th>
                <input type='checkbox' checked={checked} onClick={() => {

                    // Check or uncheck all things.
                    if (checked) {
                        setBulkSelections(null);
                    } else {
                        const newSels = {};
                        allContent.forEach(e => newSels[e.id] = true);
                        setBulkSelections(newSels);
                    }

                }} />
            </th>,
            <th>
                File
            </th>
        ].concat(heads);
    }

    const getSelectedCount = () => {
        if (!bulkSelections) {
            return 0;
        }
        var c = 0;
        for (var k in bulkSelections) {
            c++;
        }
        return c;
    }

    const renderEntry = (entry : Upload) => {
        var id = `upload_${entry.id}`;
        var parsedRef = fileRef.parse(entry.ref as string);

        if (!parsedRef) {
            return null;
        }

        var focalX = parsedRef.focalX || 50;
        var focalY = parsedRef.focalY || 50;
        var url = fileRef.getUrl(parsedRef);
        var isImage = parsedRef.isImage();
        var isVideo = parsedRef.isVideo(false);
        var checked = bulkSelections ? !!bulkSelections[entry.id] : false;
        var checkbox = <>
            <input type='checkbox' className="btn-check" checked={checked} id={id} autoComplete="off" onChange={() => {
                const newSels = { ...bulkSelections };

                if (newSels[entry.id]) {
                    delete newSels[entry.id];
                } else {
                    newSels[entry.id] = true;
                }

                setBulkSelections(newSels);
            }} />
        </>;

        return <>
            <div className="media-center__list-item" title={entry.originalName}>
                {checkbox}
                <label className="btn btn-outline-secondary" htmlFor={id}>
                    {showRef(entry.ref)}
                    <span className="media-center__id badge bg-secondary rounded-pill">
                        {entry.usageCount && entry.usageCount > 0 &&
                            <span className="media-center__usage">
                                {entry.usageCount}
                            </span>
                        }
                        {entry.id}
                    </span>
                </label>

                {/* allow image properties (such as focal point) to be set */}
                    <button type="button" className="btn btn-sm btn-primary media-center__original-filename" data-clamp="2"
                    onClick={() => {
                        setUploadModal({
                            bulkUploaded: false,
                            existingFileRef: entry.ref,
                            uploadMode: UPLOAD_SINGLE,
                            uploadId: entry.id,
                            focalX: focalX,
                            focalY: focalY,
                            alt: entry.alt,
                            author: entry.author,
                            originalName: entry.originalName,
                            transcodeState: entry.transcodeState,
                            tags: entry.tags
                        });
                    }}>
                        {`Edit - `}{entry.originalName}
                    </button>

            </div>
        </>;
    }

    const renderBulkOptions = (selectedCount : number) => {
        var message = (selectedCount > 1) ? `${selectedCount} items selected` : `1 item selected`;

        return <div className="admin-page__footer-actions">
            <span className="admin-page__footer-actions-label">
                {message}
            </span>
            <button type="button" className="btn btn-danger" onClick={() => startDelete()}>
                {`Delete selected`}
            </button>
        </div>;
    }

    const startDelete = () => {
        setConfirmDelete(true);
    }

    const cancelDelete = () => {
        setConfirmDelete(false);
    }

    const doConfirmDelete = () => {
        setConfirmDelete(false);
        setDeleting(true);
        setDeleteFailed(false);

        // get the item IDs:
        var ids = Object.keys(bulkSelections!);

        var deletes = ids.map(id => uploadApi.delete(parseInt(id) as uint));

        Promise.all(deletes).then(response => {
            setBulkSelections(null);
            setDeleteCount(deleteCount + 1);
        }).catch(e => {
            console.error(e);
            setDeleting(false);
            setDeleteFailed(true);
        });
    }

    const renderConfirmDelete = (count : number) => {
        return <ConfirmModal confirmCallback={() => doConfirmDelete()} confirmVariant="danger" cancelCallback={() => cancelDelete()}>
            <p>
                {`Are you sure you want to delete ${count} item(s)?`}
            </p>
        </ConfirmModal>
    }

    const showUploadModal = (uploadMode : number) => {
        setUploadModal({
            uploadMode: uploadMode,
            bulkUploaded: false,
            focalX: 50,
            focalY: 50
        });
    }

    const cancelUpload = () => {
        setUploadModal(null);
    }

    const saveUpload = () => {
        if (!uploadModal) {
            return;
        }

        const { focalX, focalY, alt, author, tags } = uploadModal;

        uploadApi.update(uploadModal.uploadId!,
            {
                focalX: focalX as int,
                focalY: focalY as int,
                'alt': alt,
                'author': author,
                tags: tags ? tags.map(obj => obj.id) : null
            }
        ).then(() => {
            setUploadModal(null);
        });
    }

    const renderUploadModal = () => {
        if (!uploadModal) {
            return null;
        }

        const {
            bulkUploaded,
            uploadMode,
            existingFileRef,
            focalX,
            focalY
        } = uploadModal;

        var url = '';
        var title = `Upload media`;
        var isImage = false;
        var isVideo = false;
        var isNewMedia = true;

        if (existingFileRef) {
            var parsedRef = fileRef.parse(existingFileRef);

            if (parsedRef) {
                isNewMedia = false;
                url = fileRef.getUrl(parsedRef) || '';
                isImage = parsedRef.isImage();
                isVideo = parsedRef.isVideo(false);
            }

            title = `Edit - ` + uploadModal.originalName;
        } else if(uploadMode == UPLOAD_MULTIPLE) {
            title = `Bulk upload media`;
        }

        return <>
            <Modal visible isExtraLarge title={title}
                onClose={() => {
                    if (uploadMode == UPLOAD_MULTIPLE && bulkUploaded) {
                        window.location.reload();
                    } else {
                        cancelUpload();
                    }
                }}
                className="media-center__upload-modal">
                <div className="media-center__upload-modal-internal">
                    <Container>
                        <Row>
                            {!isNewMedia && <>
                                <Column sizeMd='6'>
                                    <div className='media-center__preview-wrapper'>
                                        <div className="media-center__preview"
                                            onClick={(e) => {
                                                var imagePreviewRect = (e.target as HTMLDivElement).getBoundingClientRect();
                                                const newFx = CLOSEST_MULTIPLE * Math.round((e.offsetX / imagePreviewRect.width * 100) / CLOSEST_MULTIPLE);
                                                const newFy = CLOSEST_MULTIPLE * Math.round((e.offsetY / imagePreviewRect.height * 100) / CLOSEST_MULTIPLE);

                                                setUploadModal({...uploadModal, focalX: newFx, focalY: newFy});
                                            }}>
                                            {showRef(uploadModal.existingFileRef as string, PREVIEW_SIZE)}
                                            {isImage && !isVideo && <>
                                                <div className="media-center__preview-crosshair" style={{
                                                    left: focalX + '%',
                                                    top: focalY + '%'
                                                }}></div>
                                            </>}
                                        </div>
                                    </div>
                                </Column>
                            </>}
                            {isNewMedia && <>
                                <Uploader
                                    multiple={uploadMode == UPLOAD_MULTIPLE ? true : undefined}
                                    onUploaded={(e) => {

                                        if (uploadMode == UPLOAD_MULTIPLE) {
                                            setUploadModal({
                                                ...uploadModal, bulkUploaded: true
                                            });
                                            return;
                                        }

                                        if (!e.result.isImage) {
                                            window.location.reload();
                                            return;
                                        }
                                        setUploadModal({
                                            ...uploadModal,
                                            focalX: 50,
                                            focalY: 50,
                                            existingFileRef: e.result.ref,
                                            uploadId: e.result.id,
                                        });
                                    }} />
                            </>}
                            {!isNewMedia && <>
                                <Column sizeMd='6'>
                                    <div className="media-center__metadata">

                                        {isImage &&
                                            <>
                                            <div className="media-center__transcode">
                                                {`Transcode Status`}:{uploadModal.transcodeState}
                                            </div>

                                        <div className="form-text media-center__alt">
                                                <Input type="text" label={`Author/Photographer`} value={uploadModal.author} onChange={e => {
                                                    const input = (e.target as HTMLInputElement);
                                                    setUploadModal({ ...uploadModal, author: input.value });
                                            }} />
                                        </div>

                                        <div className="form-text media-center__alt">
                                                <Input type="text" label={`Alternative Text`} value={uploadModal.alt} onChange={e => {
                                                    const input = (e.target as HTMLInputElement);
                                                    setUploadModal({ ...uploadModal, alt: input.value });
                                            }} />
                                        </div>
                                            </>
                                        }

                                        {isImage && !isVideo &&
                                            <div className="form-text media-center__focal-point">
                                                <button type="button" className="btn btn-sm btn-outline-secondary me-2" onClick={() => {
                                                    setUploadModal({ ...uploadModal, focalX: 50, focalY: 50 });
                                                }}>
                                                    <i className="fal fa-fw fa-sync"></i>
                                                </button>
                                                {focalX}%, {focalY}%
                                            </div>
                                        }

                                        <MultiSelect value={uploadModal.tags} contentType='tag' field='name' label={`Folders`} showCreateOrEditModal={true}
                                            onChange={e => {
                                                setUploadModal({ ...uploadModal, tags: e.fullValue });
                                            }}>
                                        </MultiSelect>

                                    </div>

                                </Column>
                            </>}
                        </Row>
                    </Container>
                </div>

                <footer className="media-center__upload-modal-footer">
                    {!isNewMedia && <>
                        <a href={url} target="_blank" className="btn btn-secondary">
                            <i className="fa-fw fal fa-external-link"></i> {`Preview`}
                        </a>
                    </>}
                    {isNewMedia && <>&nbsp;</>}
                    <div className="media-center__upload-modal-footer-options">

                        {isNewMedia && <>
                            <button type="button" className="btn btn-primary" onClick={() => {

                                if (uploadMode == UPLOAD_MULTIPLE && bulkUploaded) {
                                    window.location.reload();
                                } else {
                                    cancelUpload();
                                }

                            }}>
                                {`Close`}
                            </button>
                        </>}
                        {!isNewMedia && <>
                            <button type="button" className="btn btn-outline-primary" onClick={() => cancelUpload()}>
                                {`Cancel`}
                            </button>
                            <button type="button" className="btn btn-primary" onClick={() => saveUpload()}>
                                {`Save`}
                            </button>
                        </>}
                    </div>
                </footer>
            </Modal>
        </>;
    }
    
    var selectedCount = getSelectedCount();
    
    return <>
		<SubHeader title={`Uploads`} breadcrumbs={[
			{
				title: `Uploads`
			}
		]} onQuery={(where, query) => {
			setSearchFilter(query);
		}}/>
		<div className="admin-page__content">
			<div className="admin-page__internal">
                {renderTags(uploads)}
                <div className="media-center__list">
                    {uploads ? uploads.map(renderEntry) : <Loading />}
				</div>
				{confirmDelete && renderConfirmDelete(selectedCount)}
				{uploadModal && renderUploadModal()}
			</div>
			<footer className="admin-page__footer">
				{selectedCount > 0 ? renderBulkOptions(selectedCount) : null}
				<button type="button" className="btn btn-primary" onClick={() => showUploadModal(UPLOAD_SINGLE)}>
					{`Upload`}
				</button>
				<button type="button" className="btn btn-primary" onClick={() => showUploadModal(UPLOAD_MULTIPLE)}>
					{`Bulk upload`}
				</button>
			</footer>
		</div>
    </>;
}

export default MediaCenter;